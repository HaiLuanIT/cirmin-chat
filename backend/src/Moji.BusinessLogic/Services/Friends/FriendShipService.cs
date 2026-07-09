using System.Net;
using FluentValidation;
using Moji.BusinessLogic.Exceptions;
using Moji.Contracts.Models.FriendShips;
using Moji.Contracts.Models.FriendShips.AddFriend;
using Moji.DataAccess.Commons.Constants;
using Moji.DataAccess.Commons.DbTransactionManagers;
using Moji.DataAccess.Entities;
using Moji.DataAccess.Repositories;

namespace Moji.BusinessLogic.Services.Friends;

public class FriendShipService : IFriendShipService
{
    private readonly IFriendShipRepository _friendShipRepository;
    private readonly IUserRepository _userRepository;
    private readonly IConversationRepository _conversationRepository;
    private readonly IDbTransactionManager _dbTransactionManager;
    private readonly IValidator<AddFriendRequest> _friendRequestValidator;

    public FriendShipService(IFriendShipRepository friendShipRepository, IUserRepository userRepository,
        IConversationRepository conversationRepository, IDbTransactionManager dbTransactionManager, IValidator<AddFriendRequest> friendRequestValidator)
    {
        _friendRequestValidator = friendRequestValidator;
        _friendShipRepository = friendShipRepository;
        _userRepository = userRepository;
        _conversationRepository = conversationRepository;
        _dbTransactionManager = dbTransactionManager;
    }

    public async Task AddFriend(Guid currentUserId, AddFriendRequest request)
    {
        var validationResult = await _friendRequestValidator.ValidateAsync(request);
        if (!validationResult.IsValid)
        {
            throw new MojiValidationException(validationResult.Errors);
        }
        
        if (currentUserId == request.ReceiverId) throw new MojiBadRequestException("Không thể kết bạn với bản thân");
        //check receiver exist
        var receiver = await _userRepository.FindByIdAsync(request.ReceiverId);
        if (receiver == null) throw new MojiNotFoundException("Người dùng nhận lời mời không tồn tại!");

        //check friend request is exist or not
        var friendRequest = await _friendShipRepository.FindRequestAsync(currentUserId, request.ReceiverId);
        if (friendRequest != null)
        {
            throw new MojiConflictException("Lời mời kết bạn hoặc mối quan hệ giữa hai người đã tồn tại!");
        }

        //add to db
        var newRequest = new FriendShip()
        {
            UserLeftId = currentUserId,
            UserRightId = request.ReceiverId,
            Message = request.Message,
            RequesterId = currentUserId
        };
        _friendShipRepository.Add(newRequest);
        await _dbTransactionManager.SaveChangesAsync();
    }

    public async Task ProcessFriendRequest(Guid currentUserId, Guid friendRequestId, bool isAccepted)
    {
        //check friend request exist and status must be pending
        var friendRequest = await _friendShipRepository.FindByIdAsync(friendRequestId);
        if (friendRequest == null) throw new MojiNotFoundException("Friend request not found");

        //check permission with friend request
        if (friendRequest.RequesterId == currentUserId)
        {
            throw new MojiForbiddenException("Bạn không có quyền thực hiện hành động này!");
        }

        if (friendRequest.Status != FriendShipStatus.Pending)
            throw new MojiBadRequestException("Lời mời kết bạn này đã được xử lý");

        //normalize status in request must be match in enum
        var normalizeStatus = isAccepted
            ? FriendShipStatus.Accept
            : FriendShipStatus.Reject;

        //update in db
        friendRequest.Status = normalizeStatus;

        if (normalizeStatus == FriendShipStatus.Reject)
        {
            _friendShipRepository.Delete(friendRequest);
            await _dbTransactionManager.SaveChangesAsync();
            return;
        }

        await using var transaction = await _dbTransactionManager.BeginTransactionAsync();
        try
        {
            _friendShipRepository.Update(friendRequest);
            
            var conversation = new Conversation()
            {
                Name = null,
                IsGroup = false
            };
            conversation.AddMember(friendRequest.UserLeftId);
            conversation.AddMember(friendRequest.UserRightId);
            _conversationRepository.Add(conversation);
            
            await _dbTransactionManager.SaveChangesAsync();
            await _dbTransactionManager.CommitAsync();
        }
        catch (Exception e)
        {
            await _dbTransactionManager.RollbackAsync();
            throw;
        }
        
    }

    public async Task<List<FriendResponse>> GetFriendList(Guid userId)
    {
        var friendList = await _friendShipRepository.GetListFriend(userId);
        return friendList;
    }

    public async Task<FriendRequestListResponse> GetFriendRequestList(Guid userId)
    {
        var requestInbound = await _friendShipRepository.GetInboundRequestsAsync(userId);
        var requestOutbound = await _friendShipRepository.GetOutboundRequestsAsync(userId);
        var result = new FriendRequestListResponse
        {
            Inbound = requestInbound,
            Outbound = requestOutbound
        };
        return result;
    }

    public async Task<bool> IsFriend(Guid userId, Guid friendId)
    {
        if (userId == friendId) throw new MojiBadRequestException("Không thể làm bạn với bản thân");
        return await _friendShipRepository.IsFriend(userId, friendId);
    }

    public async Task<List<string>> GetFriendIds(Guid currentUserId)
    {
        var result = await _friendShipRepository.GetFriendIds(currentUserId);
        return result.Select(x => x.ToString()).ToList();
    }
}