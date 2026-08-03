namespace Moji.BusinessLogic.Services.Auth;

public interface IAccessTokenValidator
{
    Task<bool> IsValidAsync(Guid userId, int tokenAuthVersion, CancellationToken cancellationToken);
}