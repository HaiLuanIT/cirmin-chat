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

    public async Task AddAsync(FriendShip friendShip)
    {
        await _context.Friendships.AddAsync(friendShip);
        await _context.SaveChangesAsync();
    }

    public async Task<FriendShip> FindRequestAsync(Guid userLeftId, Guid userRightId)
    {
        return await _context.Friendships
            .FirstOrDefaultAsync(x => x.UserLeftId == userLeftId && x.UserRightId == userRightId);
    }

    public async Task<FriendShip> FindByIdAsync(Guid id)
    {
        return await _context.Friendships.FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<bool> UpdateStatus(FriendShip friendShip, string status)
    {
        friendShip.Status = status;
        _context.Friendships.Update(friendShip);
        return await _context.SaveChangesAsync() > 0;
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
}