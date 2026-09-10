using CirMin.DataAccess.Entities;
using CirMin.DataAccess.Models;

namespace CirMin.DataAccess.Repositories;

public interface IUserTokenRepository
{
    Task Add(UserToken userToken);

    Task<UserToken?> FindByTokenAsync(string token);

    void RevokeToken(UserToken token);

    Task RevokeAllTokensAsync(Guid userId);

    Task<UpdatedResult> RevokeTokenAsync(UserToken token);
}