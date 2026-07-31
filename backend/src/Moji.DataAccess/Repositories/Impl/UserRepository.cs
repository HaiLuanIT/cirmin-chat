using Microsoft.EntityFrameworkCore;
using Moji.Contracts.Models.Paginations.OffsetPagination;
using Moji.DataAccess.Configurations;
using Moji.DataAccess.Entities;
using Moji.DataAccess.Models;

namespace Moji.DataAccess.Repositories.Impl;

public class UserRepository : IUserRepository
{
    private readonly ApplicationDbContext _context;

    public UserRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public void Add(User user)
    {
        _context.Users.Add(user);
    }

    public async Task<User?> FindByUsernameAsync(string username)
    {
        return await _context.Users.AsNoTracking().FirstOrDefaultAsync(x => x.Username == username);
    }

    public async Task<User?> FindByIdAsync(Guid id)
    {
        return await _context.Users.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<bool> IsEmailUniqueAsync(string email)
    {
        return !await _context.Users.AnyAsync(x => x.Email == email);
    }

    public async Task<OffsetPagingResult<UserSearchProjection>> SearchUserByUsername(Guid currentUserId,
        string username,
        int pageNumber, int pageSize)
    {
        var baseQuery = _context.Users
            .AsNoTracking()
            .Where(x => x.Id != currentUserId && x.Username.StartsWith(username));

        var count = await baseQuery.CountAsync();
        var items = await baseQuery
            .Select(x => new UserSearchProjection
            {
                Id = x.Id,
                Username = x.Username,
                AvatarUrl = x.AvatarUrl,
                DisplayName = x.FullName
            })
            .OrderBy(u => username)
            .ThenBy(u => u.Id)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();


        return new OffsetPagingResult<UserSearchProjection>
        {
            Items = items,
            TotalCount = count,
            PageNumber = pageNumber,
            PageSize = pageSize
        };
    }

    public async Task<AvatarUpdateSnapshot?> GetAvatarUpdateSnapshot(Guid userId, CancellationToken cancellationToken)
    {
        return await _context.Users.Select(user => new AvatarUpdateSnapshot
            {
                Id = user.Id,
                AvatarUrl = user.AvatarUrl,
                AvatarId = user.AvatarId,
                RowVersion = user.RowVersion
            })
            .FirstOrDefaultAsync(user => user.Id == userId, cancellationToken);
    }

    public async Task<(AvatarUpdatedResult, DateTimeOffset)> TryUpdateAvatar(Guid userId, uint expectedVersion,
        string newAvatarUrl,
        string newAvatarId, CancellationToken cancellationToken)
    {
        var now = DateTimeOffset.UtcNow;
        var result = await _context.Users.Where(user => user.Id == userId && user.RowVersion == expectedVersion)
            .ExecuteUpdateAsync(setters =>
                setters.SetProperty(user => user.AvatarId, newAvatarId)
                    .SetProperty(user => user.AvatarUrl, newAvatarUrl)
                    .SetProperty(user => user.UpdatedAt, now), cancellationToken);
        return (result == 1 ? AvatarUpdatedResult.Updated : AvatarUpdatedResult.ConcurrencyConflict, now);
    }

    public async Task UpdateUserInfo(User user,
        CancellationToken cancellationToken)
    {
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<User?> GetTrackedUser(Guid userId, CancellationToken cancellationToken)
    {
        return await _context.Users.FirstOrDefaultAsync(x => x.Id == userId && x.IsDeleted == false, cancellationToken);
    }
}