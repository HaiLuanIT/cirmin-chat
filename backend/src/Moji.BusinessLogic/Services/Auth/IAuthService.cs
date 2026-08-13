using Moji.Contracts.Models.Auth;
using Moji.Contracts.Models.Auth.ChangePassword;
using Moji.Contracts.Models.Auth.Login;
using Moji.Contracts.Models.Auth.Register;

namespace Moji.BusinessLogic.Services.Auth;

public interface IAuthService
{
    Task SignUp(RegisterRequest request);

    Task<AuthResponse> SignIn(LoginRequest request);

    Task RevokeRefreshToken(string token);

    Task<UserModel> GetUser(Guid id, CancellationToken cancellationToken);

    Task<AuthResponse> RefreshToken(string oldToken, CancellationToken cancellationToken);

    Task ChangePassword(Guid userId, ChangePasswordRequest request, CancellationToken cancellationToken);
}