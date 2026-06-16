using System.Net;
using Moji.BusinessLogic.Exceptions;
using Moji.BusinessLogic.Models.FriendShips;
using Moji.DataAccess.Commons.Constants;
using Moji.DataAccess.Entities;
using Moji.DataAccess.Repositories;
using Moji.DataAccess.Repositories.Models;

namespace Moji.BusinessLogic.Services.Friends;

public class FriendShipService : IFriendShipService
{
    private readonly IFriendShipRepository _friendShipRepository;
    private readonly IUserRepository _userRepository;

    public FriendShipService(IFriendShipRepository friendShipRepository, IUserRepository userRepository)
    {
        _friendShipRepository = friendShipRepository;
        _userRepository = userRepository;
    }


    private (Guid, Guid) NormalizeRelationShip(Guid userA, Guid userB)
    {
        if (userA == userB) throw new MojiBadRequestException("Cannot be friend with yourself");
        var isALessThanB = userA.CompareTo(userB) < 0;

        var userLeft = isALessThanB ? userA : userB;
        var userRight = isALessThanB ? userB : userA;
        return (userLeft, userRight);
    }

    public async Task AddFriend(Guid currentUserId, Guid receiverId, string message)
    {
        var (userLeft, userRight) = NormalizeRelationShip(currentUserId, receiverId);

        //check receiver exist
        var receiver = await _userRepository.FindByIdAsync(receiverId);
        if (receiver == null) throw new MojiNotFoundException("Người dùng nhận lời mời không tồn tại!");

        //check friend request is exist or not
        var friendRequest = await _friendShipRepository.FindRequestAsync(userLeft, userRight);
        if (friendRequest != null)
        {
            throw new MojiConflictException("Lời mời kết bạn hoặc mối quan hệ giữa hai người đã tồn tại!");
        }

        //add to db
        var newRequest = new FriendShip()
        {
            UserLeftId = userLeft,
            UserRightId = userRight,
            Message = message,
            UpdatedAt = DateTime.UtcNow,
            RequesterId = currentUserId
        };
        await _friendShipRepository.AddAsync(newRequest);
    }

    public async Task<bool> ResponseFriendRequest(Guid currentUserId, Guid friendRequestId, string status)
    {
        //check friend request exist and status must be pending
        var friendRequest = await _friendShipRepository.FindByIdAsync(friendRequestId);
        if (friendRequest == null) throw new MojiNotFoundException("Friend request not found");

        //check permission with friend request
        if (friendRequest.RequesterId != currentUserId)
        {
            throw new MojiForbiddenException("Bạn không có quyền thực hiện hành động này!");
        }

        if (friendRequest.Status != FriendShipStatus.Pending)
            throw new MojiBadRequestException("Lời mời kết bạn này đã được xử lý");

        //normalize status in request must be match in enum
        var normalizeStatus = status == FriendShipStatus.Accept
            ? FriendShipStatus.Accept
            : FriendShipStatus.Reject;

        //update in db
        friendRequest.Status = normalizeStatus;
        friendRequest.UpdatedAt = DateTime.UtcNow;
        var result = await _friendShipRepository.UpdateStatus(friendRequest, normalizeStatus);

        return result;
    }

    public async Task<List<FriendResponse>> GetFriendList(Guid userId)
    {
        var friendList = await _friendShipRepository.GetListFriend(userId);
        var result = friendList.Select(x => new FriendResponse(
            x.FriendId, x.fullName, x.avatarUrl, x.status
        )).ToList();
        return result;
    }

    public async Task<FriendRequestListResponse> GetFriendRequestList(Guid userId)
    {
        var requestInbound = await _friendShipRepository.GetInboundRequestsAsync(userId);
        var requestOutbound = await _friendShipRepository.GetOutboundRequestsAsync(userId);
        
        //map
        var requestInboundResponse = requestInbound.Select(x => MapToFriendRequestResponse(x)).ToList();
        var requestOutboundResponse = requestOutbound.Select(x => MapToFriendRequestResponse(x)).ToList();

        var result = new FriendRequestListResponse(requestInboundResponse, requestOutboundResponse);
        return result;
    }

    //helper
    private FriendRequestResponse MapToFriendRequestResponse(FriendshipRawData rawData)
    {
        var response = new FriendRequestResponse(rawData.RequestId, rawData.FriendId, rawData.fullName,
            rawData.avatarUrl, rawData.status);
        return response;
    }
}