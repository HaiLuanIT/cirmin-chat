using Moji.Contracts.Models.Auth;
using Moji.Contracts.Models.Auth.Login;
using Moji.Contracts.Models.Auth.Register;

namespace Moji.BusinessLogic.Services.Auth;

public interface IAuthService
{
    Task SignUp(RegisterRequest request);
    
    Task<AuthResponse> SignIn(LoginRequest request);
    
    Task RevokeRefreshToken(string token);

    Task<UserModel> GetUser(Guid id);
    
    Task<AuthResponse> RefreshToken(string oldToken);
}