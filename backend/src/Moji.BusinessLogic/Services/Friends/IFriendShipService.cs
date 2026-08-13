using Moji.Contracts.Models.FriendShips;
using Moji.Contracts.Models.FriendShips.AddFriend;

namespace Moji.BusinessLogic.Services.Friends;

public interface IFriendShipService
{
    Task AddFriend(Guid currentUserId, AddFriendRequest request, CancellationToken cancellationToken);

    Task ProcessFriendRequest(Guid currentUserId, Guid friendRequestId, bool isAccepted);
    
    Task<List<FriendResponse>> GetFriendList(Guid userId);

    Task<FriendRequestListResponse> GetFriendRequestList(Guid userId);
    
    Task<bool> IsFriend(Guid userId, Guid friendId);

    Task<List<string>> GetFriendIds(Guid currentUserId);
}