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
    
    public void Add(UserToken userToken)
    {
         _context.UserTokens.AddAsync(userToken);
    }

    public async Task<UserToken?> FindByTokenAsync(string token)
    {
        return await _context.UserTokens.FirstOrDefaultAsync(x => x.Token == token);
    }

    public void RevokeToken(UserToken token)
    {
        _context.UserTokens.Update(token);
    }
}