using Moji.DataAccess.Entities;

namespace Moji.DataAccess.Repositories;

public interface IUserRepository
{
    void Add(User user);
    
    Task<User?> FindByUsernameAsync(string username);
    
    Task<User?> FindByIdAsync(Guid id);
    
    Task<bool> IsEmailUniqueAsync(string email);
    
}