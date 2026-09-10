using CirMin.BusinessLogic.Constants;
using CirMin.BusinessLogic.Exceptions;
using CirMin.BusinessLogic.Services.Storage;
using FluentValidation;
using CirMin.Contracts.Errors;
using CirMin.Contracts.Models.Paginations.OffsetPagination;
using CirMin.Contracts.Models.Users.SearchUser;
using CirMin.Contracts.Models.Users.UpdateUserInfo;
using CirMin.Contracts.Models.Users.UploadAvatar;
using CirMin.DataAccess.Commons.Constants;
using CirMin.DataAccess.Commons.DbTransactionManagers;
using CirMin.DataAccess.Entities;
using CirMin.DataAccess.Models;
using CirMin.DataAccess.Repositories;

namespace CirMin.BusinessLogic.Services.Users;

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
        if (!validationResult.IsValid) throw new CirMinValidationException(validationResult.Errors);

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
        if (user == null) throw new CirMinNotFoundException(ErrorCodes.User.NotFound);

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
                throw new CirMinConflictException(ErrorCodes.Concurrency.Conflict);
            }

            updatedAt = updateResult.Item2;
        }
        catch (CirMinConflictException)
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
        if (!validationResult.IsValid) throw new CirMinValidationException(validationResult.Errors);

        //validate user exist
        var user = await _userRepository.FindByIdAsync(currentUserId, cancellationToken);
        if (user == null) throw new CirMinNotFoundException(ErrorCodes.User.NotFound);

        //update
        if (normalizeRequest.Email != null)
        {
            var isEmailUnique = await _userRepository.IsEmailUniqueAsync(normalizeRequest.Email);
            if (!isEmailUnique)
                if (user.Email != normalizeRequest.Email)
                    throw new CirMinConflictException(ErrorCodes.User.EmailAlreadyExists);
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
        var originalUser = new UpdateUserInfoSnapShot
        {
            DisplayName = user.FullName,
            Bio = user.Bio,
            Email = user.Email
        };
        var result = await _userRepository.UpdateUserInfo(user, normalizeRequest.DisplayName, normalizeRequest.Bio,
            normalizeRequest.Email, cancellationToken);
        var updatedResultSnapshot = new UpdateUserInfoSnapShot();
        if (result.Item1 == UpdatedResult.DuplicatedEmail)
            throw new CirMinConflictException(ErrorCodes.User.EmailAlreadyExists);
        if (result.Item1 == UpdatedResult.Updated)
        {
            updatedResultSnapshot =
                result.Item2 ?? throw new InvalidOperationException("Success update must contain snapshot");
        }
        // if conflict retry 2 times
        else if (result.Item1 == UpdatedResult.ConcurrencyConflict)
        {
            var isUpdated = false;
            for (var i = 0; i < 2; i++)
            {
                var currentUser = await _userRepository.FindByIdAsync(currentUserId, cancellationToken);
                if (currentUser == null) throw new CirMinNotFoundException(ErrorCodes.User.NotFound);
                if (currentUser.RowVersion != user.RowVersion)
                {
                    //check if field need update not be changed by other user
                    var hasSemanticConflict =
                        RequestedDisplayNameChangedDifferently(currentUser, originalUser, normalizeRequest) ||
                        RequestedBioChangedDifferently(currentUser, originalUser, normalizeRequest) ||
                        RequestedEmailChangedDifferently(currentUser, originalUser, normalizeRequest);

                    if (hasSemanticConflict) throw new CirMinConflictException(ErrorCodes.Concurrency.Conflict);

                    //check if operation already satisfied/duplicate request
                    var operationAlreadySatisfied =
                        (normalizeRequest.DisplayName == null ||
                         normalizeRequest.DisplayName == currentUser.FullName) &&
                        (normalizeRequest.Bio == null || normalizeRequest.Bio == currentUser.Bio) &&
                        (normalizeRequest.Email == null || normalizeRequest.Email == currentUser.Email);
                    if (operationAlreadySatisfied)
                    {
                        isUpdated = true;
                        updatedResultSnapshot = new UpdateUserInfoSnapShot
                        {
                            DisplayName = currentUser.FullName,
                            Bio = currentUser.Bio,
                            Email = currentUser.Email
                        };
                        break;
                    }

                    var retryUpdateResult = await _userRepository.UpdateUserInfo(currentUser,
                        normalizeRequest.DisplayName,
                        normalizeRequest.Bio,
                        normalizeRequest.Email, cancellationToken);
                    if (retryUpdateResult.Item1 == UpdatedResult.Updated)
                    {
                        isUpdated = true;
                        updatedResultSnapshot = retryUpdateResult.Item2 ??
                                                throw new InvalidOperationException(
                                                    "Success update must contain snapshot");
                        break;
                    }

                    if (retryUpdateResult.Item1 == UpdatedResult.DuplicatedEmail)
                        throw new CirMinConflictException(ErrorCodes.User.EmailAlreadyExists);
                }
            }

            if (!isUpdated)
                throw new CirMinConflictException(ErrorCodes.Concurrency.Conflict);
        }


        //map
        var model = new UpdateUserInfoResponse
        {
            DisplayName = updatedResultSnapshot.DisplayName,
            Bio = updatedResultSnapshot.Bio,
            Email = updatedResultSnapshot.Email
        };
        return model;
    }

    private bool RequestedDisplayNameChangedDifferently(User currentUser, UpdateUserInfoSnapShot originalUser,
        UpdateUserInfoRequest normalizeRequest)
    {
        return normalizeRequest.DisplayName is not null && currentUser.FullName != originalUser.DisplayName &&
               currentUser.FullName != normalizeRequest.DisplayName;
    }

    private bool RequestedBioChangedDifferently(User currentUser, UpdateUserInfoSnapShot originalUser,
        UpdateUserInfoRequest normalizeRequest)
    {
        return normalizeRequest.Bio is not null &&
               currentUser.Bio != originalUser.Bio && currentUser.Bio != normalizeRequest.Bio;
    }

    private bool RequestedEmailChangedDifferently(User currentUser, UpdateUserInfoSnapShot originalUser,
        UpdateUserInfoRequest normalizeRequest)
    {
        return normalizeRequest.Email is not null && currentUser.Email != originalUser.Email &&
               currentUser.Email != normalizeRequest.Email;
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