using Microsoft.EntityFrameworkCore;
using Moji.DataAccess.Configurations;
using Moji.DataAccess.Entities;

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
}