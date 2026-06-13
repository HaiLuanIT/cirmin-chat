using Moji.DataAccess.Entities;

namespace Moji.DataAccess.Repositories;

public interface IFriendShipRepository
{
    Task AddAsync(FriendShip friendShip);
}