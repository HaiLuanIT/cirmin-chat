using CirMin.Contracts.Models.Auth;
using CirMin.Contracts.Models.Auth.ChangePassword;
using CirMin.Contracts.Models.Auth.Login;
using CirMin.Contracts.Models.Auth.Register;

namespace CirMin.BusinessLogic.Services.Auth;

public interface IAuthService
{
    Task SignUp(RegisterRequest request);

    Task<AuthResponse> SignIn(LoginRequest request);

    Task RevokeRefreshToken(string token);

    Task<UserModel> GetUser(Guid id, CancellationToken cancellationToken);

    Task<AuthResponse> RefreshToken(string oldToken, CancellationToken cancellationToken);

    Task ChangePassword(Guid userId, ChangePasswordRequest request, CancellationToken cancellationToken);
}