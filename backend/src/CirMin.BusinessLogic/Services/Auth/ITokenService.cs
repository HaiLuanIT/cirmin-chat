using CirMin.DataAccess.Entities;

namespace CirMin.BusinessLogic.Services.Auth;

public interface ITokenService
{
    string GenerateAccessToken(User user, int authVersion);
    string GenerateRefreshToken();
}