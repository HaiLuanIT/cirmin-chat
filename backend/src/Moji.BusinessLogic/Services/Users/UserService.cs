using FluentValidation;
using Moji.BusinessLogic.Constants;
using Moji.BusinessLogic.Exceptions;
using Moji.BusinessLogic.Services.Storage;
using Moji.Contracts.Errors;
using Moji.Contracts.Models.Paginations.OffsetPagination;
using Moji.Contracts.Models.Users.SearchUser;
using Moji.Contracts.Models.Users.UpdateUserInfo;
using Moji.Contracts.Models.Users.UploadAvatar;
using Moji.DataAccess.Commons.Constants;
using Moji.DataAccess.Commons.DbTransactionManagers;
using Moji.DataAccess.Models;
using Moji.DataAccess.Repositories;

namespace Moji.BusinessLogic.Services.Users;

public class UserService : IUserService
{
    private readonly IConversationRepository _convoRepository;
    private readonly IFriendShipRepository _friendShipRepository;
    private readonly IImageStorageService _imageStorageService;
    private readonly IValidator<SearchUserRequest> _searchUserValidator;
    private readonly IDbTransactionManager _txManager;
    private readonly IValidator<UpdateUserInfoRequest> _updateUserInfoValidator;
    private readonly IUserRepository _userRepository;

    public UserService(IUserRepository userRepository, IValidator<SearchUserRequest> searchUserValidator,
        IFriendShipRepository friendShipRepository, IConversationRepository convoRepository,
        IImageStorageService imageStorageService, IValidator<UpdateUserInfoRequest> updateUserInfoValidator,
        IDbTransactionManager txManager)
    {
        _userRepository = userRepository;
        _searchUserValidator = searchUserValidator;
        _friendShipRepository = friendShipRepository;
        _convoRepository = convoRepository;
        _imageStorageService = imageStorageService;
        _updateUserInfoValidator = updateUserInfoValidator;
        _txManager = txManager;
    }

    public async Task<OffsetPagingResult<SearchUserResponse>> SearchByUsername(Guid currentUserId,
        SearchUserRequest request)
    {
        //check request by validator
        var validationResult = await _searchUserValidator.ValidateAsync(request);
        if (!validationResult.IsValid) throw new MojiValidationException(validationResult.Errors);

        // normalize username
        var normalizedUsername = request.Username.Trim().ToLower();

        var usersInfo = await _userRepository.SearchUserByUsername(currentUserId, normalizedUsername,
            request.PageNumber,
            request.PageSize);

        if (!usersInfo.Items.Any())
            return new OffsetPagingResult<SearchUserResponse>
            {
                PageNumber = request.PageNumber,
                PageSize = request.PageSize,
                TotalCount = 0
            };

        var userIds = usersInfo.Items.Select(x => x.Id).ToList();

        var relationLookup = await _friendShipRepository.GetUsersRelationStatus(currentUserId, userIds);

        var relationIds = relationLookup.Where(x => x.Value.Status == FriendShipStatus.Accept).Select(x => x.Key)
            .ToList();

        var directConversationOfUser = await _convoRepository.GetDirectConversationIdsByUserId(currentUserId);

        var peerConversationOfParticipant =
            await _convoRepository.GetDirectConversationIdsByParticipantIds(relationIds, directConversationOfUser);

        var response = usersInfo.Items.Select(x =>
            {
                relationLookup.TryGetValue(x.Id, out var relationStatus);

                var userRelationStatus = ResolveUserRelationStatus(currentUserId, relationStatus);

                peerConversationOfParticipant.TryGetValue(x.Id, out var conversationId);

                return new SearchUserResponse
                {
                    Id = x.Id,
                    Username = x.Username,
                    DisplayName = x.DisplayName,
                    AvatarUrl = x.AvatarUrl,
                    RelationStatus = userRelationStatus,
                    ConversationId = conversationId
                };
            })
            .ToList();
        return new OffsetPagingResult<SearchUserResponse>
        {
            PageNumber = request.PageNumber,
            PageSize = request.PageSize,
            TotalCount = usersInfo.TotalCount,
            Items = response
        };
    }

    public async Task<UploadUserAvatarResponse> UpdateUserAvatarAsync(Guid currentUserId, Stream content,
        string fileName,
        CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetAvatarUpdateSnapshot(currentUserId, cancellationToken);
        if (user == null) throw new MojiNotFoundException(ErrorCodes.User.NotFound);

        var folderName = $"users/{user.Id}/avatars";
        var uploadResult =
            await _imageStorageService.UploadImageAsync(content, fileName, folderName,
                cancellationToken);

        var oldAvatarId = user.AvatarId;
        DateTimeOffset updatedAt;
        try
        {
            var updateResult = await _userRepository.TryUpdateAvatar(currentUserId, user.RowVersion,
                uploadResult.SecureUrl,
                uploadResult.PublicId, cancellationToken);

            if (updateResult.Item1 != UpdatedResult.Updated)
            {
                var isDeleted =
                    await _imageStorageService.DeleteImageAsync(uploadResult.PublicId, CancellationToken.None);
                if (!isDeleted)
                    //todo: logg to manual delete or for background job delete cleann up
                    Console.WriteLine("Error deleting image");
                throw new MojiConflictException(ErrorCodes.Concurrency.Conflict);
            }

            updatedAt = updateResult.Item2;
        }
        catch (MojiConflictException)
        {
            throw;
        }
        catch (Exception)
        {
            var isDeleted = await _imageStorageService.DeleteImageAsync(uploadResult.PublicId, CancellationToken.None);
            if (!isDeleted)
                //todo: logg to manual delete or for background job delete cleann up
                Console.WriteLine("Error deleting image");
            throw;
        }

        if (oldAvatarId != null)
        {
            var isDeleted = await _imageStorageService.DeleteImageAsync(oldAvatarId, cancellationToken);
            if (!isDeleted)
                //todo: logg to manual delete or for background job delete cleann up
                Console.WriteLine("Error deleting image");
        }

        var uploadUserAvatarResponse = new UploadUserAvatarResponse
        {
            UserId = user.Id,
            AvatarUrl = uploadResult.SecureUrl,
            UpdatedAt = updatedAt
        };
        return uploadUserAvatarResponse;
    }

    public async Task<UpdateUserInfoResponse> UpdateUserInfoAsync(Guid currentUserId, UpdateUserInfoRequest request,
        CancellationToken cancellationToken)
    {
        //normalize request
        var normalizeRequest = new UpdateUserInfoRequest
        {
            DisplayName = NormalizeString(request.DisplayName),
            Bio = NormalizeString(request.Bio),
            Email = NormalizeString(request.Email)?.ToLower()
        };
        //validate request
        var validationResult = await _updateUserInfoValidator.ValidateAsync(normalizeRequest);
        if (!validationResult.IsValid) throw new MojiValidationException(validationResult.Errors);

        //validate user exist
        var user = await _userRepository.GetTrackedUser(currentUserId, cancellationToken);
        if (user == null) throw new MojiNotFoundException(ErrorCodes.User.NotFound);

        //update
        if (normalizeRequest.Email != null)
        {
            var isEmailUnique = await _userRepository.IsEmailUniqueAsync(normalizeRequest.Email);
            if (!isEmailUnique)
                if (user.Email != normalizeRequest.Email)
                    throw new MojiConflictException(ErrorCodes.User.EmailAlreadyExists);
        }

        //if no update return old value
        if (normalizeRequest.DisplayName == null && normalizeRequest.Bio == null && normalizeRequest.Email == null)
            return new UpdateUserInfoResponse
            {
                DisplayName = user.FullName,
                Bio = user.Bio,
                Email = user.Email
            };

        //update user info
        var result = await _userRepository.UpdateUserInfo(user, normalizeRequest.DisplayName, normalizeRequest.Bio,
            normalizeRequest.Email, cancellationToken);
        if (result == UpdatedResult.ConcurrencyConflict)
            throw new MojiConflictException(ErrorCodes.Concurrency.Conflict);
        if (result == UpdatedResult.DuplicatedEmail)
            throw new MojiConflictException(ErrorCodes.User.EmailAlreadyExists);

        //map
        var model = new UpdateUserInfoResponse
        {
            DisplayName = user.FullName,
            Bio = user.Bio,
            Email = user.Email
        };
        return model;
    }

    //helper
    private static string? NormalizeString(string? value)
    {
        return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    }

    private static string? ResolveUserRelationStatus(Guid currentUserId, UserRelationShipProjection? relationStatus)
    {
        string userRelationStatus = null;
        switch (relationStatus?.Status)
        {
            case "accept":
            {
                userRelationStatus = UserRelationStatus.Friend;
                break;
            }
            case "pending":
            {
                if (relationStatus.RequesterId == currentUserId)
                {
                    userRelationStatus = UserRelationStatus.RequestSent;
                    break;
                }

                userRelationStatus = UserRelationStatus.RequestReceived;
                break;
            }
            case "reject":
            {
                userRelationStatus = UserRelationStatus.Reject;
                break;
            }
            case "block":
            {
                userRelationStatus = UserRelationStatus.Block;
                break;
            }
        }

        return userRelationStatus;
    }
}