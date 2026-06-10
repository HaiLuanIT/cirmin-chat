using Moji.DataAccess.Entities;

namespace Moji.DataAccess.Repositories;

public interface IUserTokenRepository
{
    Task AddAsync(UserToken userToken);
    
    Task<UserToken?> FindByTokenAsync(string token);
    
    Task<bool> RevokeTokenAsync(UserToken token);
}