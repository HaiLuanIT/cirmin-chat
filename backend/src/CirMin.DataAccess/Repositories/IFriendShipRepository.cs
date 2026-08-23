using CirMin.DataAccess.Entities;
using CirMin.DataAccess.Models;
using CirMin.Contracts.Models.FriendShips;

namespace CirMin.DataAccess.Repositories;

public interface IFriendShipRepository
{
    void Add(FriendShip friendShip);

    Task<FriendShip> FindRequestAsync(Guid userLeftId, Guid userRightId);

    Task<FriendShip> FindByIdAsync(Guid id);

    void Update(FriendShip friendShip);

    void Delete(FriendShip friendShip);

    Task<List<FriendResponse>> GetListFriend(Guid userId);

    Task<List<FriendRequestResponse>> GetInboundRequestsAsync(Guid userId);

    Task<List<FriendRequestResponse>> GetOutboundRequestsAsync(Guid userId);

    Task<bool> IsFriend(Guid userId, Guid friendId);

    Task<List<Guid>> GetFriendIds(Guid userId);

    Task<Dictionary<Guid, UserRelationShipProjection>> GetUsersRelationStatus(Guid currentUserId, List<Guid> userIds);
}