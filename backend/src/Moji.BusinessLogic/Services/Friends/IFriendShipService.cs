using Moji.BusinessLogic.Models.FriendShips;

namespace Moji.BusinessLogic.Services.Friends;

public interface IFriendShipService
{
    Task AddFriend(Guid currentUserId, FriendRequestModel request);

    Task ProcessFriendRequest(Guid currentUserId, Guid friendRequestId, bool isAccepted);
    
    Task<List<FriendResponse>> GetFriendList(Guid userId);

    Task<FriendRequestListResponse> GetFriendRequestList(Guid userId);
    
    Task<bool> IsFriend(Guid userId, Guid friendId);
}