using Moji.DataAccess.Entities;

namespace Moji.DataAccess.Repositories;

public interface IUserRepository
{
    Task AddAsync(User user);
    
    Task<User?> FindByUserNameAsync(string userName);
    
    Task<User?> FindByIdAsync(Guid id);
    
    Task<bool> IsEmailUniqueAsync(string email);
    
}