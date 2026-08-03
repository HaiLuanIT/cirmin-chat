using Moji.DataAccess.Entities;

namespace Moji.BusinessLogic.Services.Auth;

public interface ITokenService
{
    string GenerateAccessToken(User user, int authVersion);
    string GenerateRefreshToken();
}