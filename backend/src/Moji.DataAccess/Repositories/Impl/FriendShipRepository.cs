using Microsoft.EntityFrameworkCore;
using Moji.DataAccess.Commons.Constants;
using Moji.DataAccess.Configurations;
using Moji.DataAccess.Entities;
using Moji.DataAccess.Repositories.Models;

namespace Moji.DataAccess.Repositories.Impl;

public class FriendShipRepository : IFriendShipRepository
{
    private readonly ApplicationDbContext _context;

    public FriendShipRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public void Add(FriendShip friendShip)
    {
        var (userLeftNormalize, userRightNormalize) = NormalizeRelationShip(friendShip.UserLeftId, friendShip.UserRightId);
        
        friendShip.UserLeftId = userLeftNormalize;
        friendShip.UserRightId = userRightNormalize;
        
         _context.Friendships.Add(friendShip);
    }

    public async Task<FriendShip> FindRequestAsync(Guid userLeftId, Guid userRightId)
    {
        var (userLeftNormalize, userRightNormalize) = NormalizeRelationShip(userLeftId, userRightId);
        return await _context.Friendships
            .FirstOrDefaultAsync(x => x.UserLeftId == userLeftNormalize && x.UserRightId == userRightNormalize);
    }

    public async Task<FriendShip> FindByIdAsync(Guid id)
    {
        return await _context.Friendships.FirstOrDefaultAsync(x => x.Id == id);
    }

    public void Update(FriendShip friendShip)
    {
        _context.Entry(friendShip).State = EntityState.Modified;
    }

    public void Delete(FriendShip friendShip)
    {
        _context.Friendships.Remove(friendShip);
    }

    public async Task<List<FriendshipRawData>> GetListFriend(Guid userId)
    {
        //if is a request, get receiver
        var asRequest = await _context.Friendships
            .Where(x => x.UserLeftId == userId && x.Status == FriendShipStatus.Accept)
            .Select(x =>
                new FriendshipRawData(x.UserRightId, x.UserRight.FullName, x.UserRight.AvatarUrl, x.Status, x.Id))
            .ToListAsync();

        //if is a receiver, get requester
        var asReceive = await _context.Friendships
            .Where(x => x.UserRightId == userId && x.Status == FriendShipStatus.Accept)
            .Select(x => new FriendshipRawData(x.UserLeftId, x.UserLeft.FullName, x.UserLeft.AvatarUrl, x.Status, x.Id))
            .ToListAsync();
        return asRequest.Concat(asReceive).ToList();
    }

    public async Task<List<FriendshipRawData>> GetInboundRequestsAsync(Guid userId)
    {
        //ds lời mời đã nhận
        var result = await _context.Friendships.AsNoTracking()
            .Where(x => (x.UserLeftId == userId || x.UserRightId == userId)
                        && x.RequesterId != userId
                        && x.Status == FriendShipStatus.Pending)
            .OrderByDescending(x => x.UpdatedAt)
            .Select(x => x.UserLeftId == userId 
                ? new FriendshipRawData(x.UserRightId, x.UserRight.FullName, x.UserRight.AvatarUrl, x.Status, x.Id)
                : new FriendshipRawData(x.UserLeftId, x.UserLeft.FullName, x.UserLeft.AvatarUrl, x.Status, x.Id))
            .ToListAsync();
        return result;
    }

    public async Task<List<FriendshipRawData>> GetOutboundRequestsAsync(Guid userId)
    {
        var result = await _context.Friendships.AsNoTracking()
            .Where(x => x.RequesterId == userId && x.Status == FriendShipStatus.Pending)
            .OrderByDescending(x => x.UpdatedAt)
            .Select(x => x.UserLeftId == userId 
                ? new FriendshipRawData(x.UserRightId, x.UserRight.FullName, x.UserRight.AvatarUrl, x.Status, x.Id)
                : new FriendshipRawData(x.UserLeftId, x.UserLeft.FullName, x.UserLeft.AvatarUrl, x.Status, x.Id))
            .ToListAsync();

        return result;
    }

    public async Task<bool> IsFriend(Guid userId, Guid friendId)
    {
        var (userLeftNormalize, userRightNormalize) = NormalizeRelationShip(userId, friendId);
        return await _context.Friendships
            .AnyAsync(x =>
                x.UserLeftId == userLeftNormalize && x.UserRightId == userRightNormalize &&
                x.Status == FriendShipStatus.Accept);
    }

    public async Task<List<Guid>> GetFriendIds(Guid userId)
    {
        //if is a request, get receiver
        var asRequest = await _context.Friendships
            .Where(x => x.UserLeftId == userId && x.Status == FriendShipStatus.Accept)
            .Select(x =>
                x.UserRightId)
            .ToListAsync();

        //if is a receiver, get requester
        var asReceive = await _context.Friendships
            .Where(x => x.UserRightId == userId && x.Status == FriendShipStatus.Accept)
            .Select(x => x.UserLeftId)
            .ToListAsync();
        return asRequest.Concat(asReceive).ToList();
    }

    //helper
    private (Guid, Guid) NormalizeRelationShip(Guid userA, Guid userB)
    {
        var isALessThanB = userA.CompareTo(userB) < 0;

        var userLeft = isALessThanB ? userA : userB;
        var userRight = isALessThanB ? userB : userA;
        return (userLeft, userRight);
    }
}