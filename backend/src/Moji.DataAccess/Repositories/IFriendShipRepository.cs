using Moji.DataAccess.Entities;
using Moji.DataAccess.Repositories.Models;

namespace Moji.DataAccess.Repositories;

public interface IFriendShipRepository
{
    Task AddAsync(FriendShip friendShip);

    Task<FriendShip> FindRequestAsync(Guid userLeftId, Guid userRightId);

    Task<FriendShip> FindByIdAsync(Guid id);

    Task<bool> UpdateStatus(FriendShip friendShip, string status);

    Task<List<FriendshipRawData>> GetListFriend(Guid userId);

    Task<List<FriendshipRawData>> GetInboundRequestsAsync(Guid userId);
    
    Task<List<FriendshipRawData>> GetOutboundRequestsAsync(Guid userId);
}