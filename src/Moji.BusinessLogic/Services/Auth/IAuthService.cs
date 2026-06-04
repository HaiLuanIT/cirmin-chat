using Moji.BusinessLogic.Models.Auth;

namespace Moji.BusinessLogic.Services.Auth;

public interface IAuthService
{
    Task SignUp(RegisterRequest request);
    
    Task<AuthResponse> SignIn(LoginRequest request);
}