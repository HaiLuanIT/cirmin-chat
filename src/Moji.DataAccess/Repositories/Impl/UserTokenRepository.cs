using Microsoft.EntityFrameworkCore;
using Moji.DataAccess.Configurations;
using Moji.DataAccess.Entities;

namespace Moji.DataAccess.Repositories.Impl;

public class UserTokenRepository : IUserTokenRepository
{
    private readonly ApplicationDbContext _context;
    public UserTokenRepository(ApplicationDbContext context)
    {
        _context = context;
    }
    
    public async Task AddAsync(UserToken userToken)
    {
        await _context.UserTokens.AddAsync(userToken);
        await _context.SaveChangesAsync();
    }

    public async Task<UserToken?> FindByTokenAsync(string token)
    {
        return await _context.UserTokens.FirstOrDefaultAsync(x => x.Token == token);
    }

    public async Task<bool> RevokeTokenAsync(UserToken token)
    {
        _context.UserTokens.Update(token);
        return await _context.SaveChangesAsync() > 0;
    }
}