using CirMin.DataAccess.Configurations;
using CirMin.DataAccess.Entities;
using CirMin.DataAccess.Models;
using Microsoft.EntityFrameworkCore;

namespace CirMin.DataAccess.Repositories.Impl;

public class UserTokenRepository : IUserTokenRepository
{
    private readonly ApplicationDbContext _context;

    public UserTokenRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task Add(UserToken userToken)
    {
        await _context.UserTokens.AddAsync(userToken);
    }

    public async Task<UserToken?> FindByTokenAsync(string token)
    {
        return await _context.UserTokens.FirstOrDefaultAsync(x => x.Token == token);
    }

    public void RevokeToken(UserToken token)
    {
        _context.UserTokens.Update(token);
    }

    public async Task RevokeAllTokensAsync(Guid userId)
    {
        var now = DateTime.UtcNow;
        await _context.UserTokens.Where(token => token.UserId == userId && !token.IsRevoked)
            .ExecuteUpdateAsync(setters =>
                setters.SetProperty(token => token.IsRevoked, true).SetProperty(token => token.UpdatedAt, now));
    }

    public async Task<UpdatedResult> RevokeTokenAsync(UserToken token)
    {
        var now = DateTimeOffset.UtcNow;

        var result = await _context.UserTokens
            .Where(ut => ut.Id == token.Id && !ut.IsRevoked && ut.AuthVersion == token.AuthVersion && ut.ExpiresAt > now)
            .ExecuteUpdateAsync(setters =>
                setters.SetProperty(ut => ut.IsRevoked, true).SetProperty(ut => ut.UpdatedAt, now));
        var updatedResult = result == 1 ? UpdatedResult.Updated : UpdatedResult.ConcurrencyConflict;
        return updatedResult;
    }
}