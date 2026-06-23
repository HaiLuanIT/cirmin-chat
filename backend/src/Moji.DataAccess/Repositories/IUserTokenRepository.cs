using Moji.DataAccess.Entities;

namespace Moji.DataAccess.Repositories;

public interface IUserTokenRepository
{
    void Add(UserToken userToken);
    
    Task<UserToken?> FindByTokenAsync(string token);
    
    void RevokeToken(UserToken token);
}