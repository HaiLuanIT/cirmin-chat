using System.Net;
using Moji.DataAccess.Repositories;

namespace Moji.BusinessLogic.Services.Friends;

public class FriendShipService
{
    private readonly IFriendShipRepository _friendShipRepository;

    public FriendShipService(IFriendShipRepository friendShipRepository)
    {
        _friendShipRepository = friendShipRepository;
    }


    private (Guid,Guid) NormalizeRelationShip(Guid userA, Guid userB)
    {
        if (userA == userB) throw new ArgumentException("Cannot be friend with yourself");
        var isALessThanB = userA.CompareTo(userB) < 0;
        
        var requester = isALessThanB ? userA : userB;
        var receiver = isALessThanB ? userB : userA;
        return (requester, receiver);
    }
}