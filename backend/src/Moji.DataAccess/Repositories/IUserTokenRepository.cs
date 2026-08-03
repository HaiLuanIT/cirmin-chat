using Moji.DataAccess.Entities;
using Moji.DataAccess.Models;

namespace Moji.DataAccess.Repositories;

public interface IUserTokenRepository
{
    Task Add(UserToken userToken);

    Task<UserToken?> FindByTokenAsync(string token);

    void RevokeToken(UserToken token);

    Task RevokeAllTokensAsync(Guid userId);

    Task<UpdatedResult> RevokeTokenAsync(UserToken token);
}