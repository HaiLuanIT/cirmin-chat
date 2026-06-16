using Moji.BusinessLogic.Models.FriendShips;

namespace Moji.BusinessLogic.Services.Friends;

public interface IFriendShipService
{
    Task AddFriend(Guid currentUserId, Guid receiverId, string message);

    Task<bool> ResponseFriendRequest(Guid currentUserId, Guid friendRequestId, string status);
    
    Task<List<FriendResponse>> GetFriendList(Guid userId);

    Task<FriendRequestListResponse> GetFriendRequestList(Guid userId);
}