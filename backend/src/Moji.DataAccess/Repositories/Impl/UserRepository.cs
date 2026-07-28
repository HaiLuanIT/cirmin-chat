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
        return await _context.Users.FirstOrDefaultAsync(x => x.Id == id);
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
}