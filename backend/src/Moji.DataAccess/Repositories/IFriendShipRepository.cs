using Moji.DataAccess.Entities;
using Moji.DataAccess.Repositories.Models;

namespace Moji.DataAccess.Repositories;

public interface IFriendShipRepository
{
    void Add(FriendShip friendShip);

    Task<FriendShip> FindRequestAsync(Guid userLeftId, Guid userRightId);

    Task<FriendShip> FindByIdAsync(Guid id);

    void Update(FriendShip friendShip);
    
    void Delete(FriendShip friendShip);

    Task<List<FriendshipRawData>> GetListFriend(Guid userId);

    Task<List<FriendshipRawData>> GetInboundRequestsAsync(Guid userId);
    
    Task<List<FriendshipRawData>> GetOutboundRequestsAsync(Guid userId);
    
    Task<bool> IsFriend(Guid userId, Guid friendId);
}