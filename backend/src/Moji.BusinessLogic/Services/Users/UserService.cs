using FluentValidation;
using Moji.BusinessLogic.Exceptions;
using Moji.Contracts.Models.Paginations.OffsetPagination;
using Moji.Contracts.Models.Users.SearchUser;
using Moji.DataAccess.Commons.Constants;
using Moji.DataAccess.Repositories;

namespace Moji.BusinessLogic.Services.Users;

public class UserService : IUserService
{
    private readonly IConversationRepository _convoRepository;
    private readonly IFriendShipRepository _friendShipRepository;
    private readonly IValidator<SearchUserRequest> _searchUserValidator;
    private readonly IUserRepository _userRepository;

    public UserService(IUserRepository userRepository, IValidator<SearchUserRequest> searchUserValidator,
        IFriendShipRepository friendShipRepository, IConversationRepository convoRepository)
    {
        _userRepository = userRepository;
        _searchUserValidator = searchUserValidator;
        _friendShipRepository = friendShipRepository;
        _convoRepository = convoRepository;
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

        var relationIds = relationLookup.Where(x => x.Value == FriendShipStatus.Accept).Select(x => x.Key)
            .ToList();

        var directConversationOfUser = await _convoRepository.GetDirectConversationIdsByUserId(currentUserId);

        var peerConversationOfParticipant =
            await _convoRepository.GetDirectConversationIdsByParticipantIds(relationIds, directConversationOfUser);

        var response = usersInfo.Items.Select(x =>
            {
                relationLookup.TryGetValue(x.Id, out var relationStatus);
                peerConversationOfParticipant.TryGetValue(x.Id, out var conversationId);

                return new SearchUserResponse
                {
                    Id = x.Id,
                    Username = x.Username,
                    DisplayName = x.DisplayName,
                    AvatarUrl = x.AvatarUrl,
                    RelationStatus = relationStatus,
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
}