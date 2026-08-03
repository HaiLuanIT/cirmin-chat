using Moji.DataAccess.Repositories;

namespace Moji.BusinessLogic.Services.Auth;

public class AccessTokenValidator : IAccessTokenValidator
{
    private readonly IUserRepository _userRepository;

    public AccessTokenValidator(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<bool> IsValidAsync(Guid userId, int tokenAuthVersion, CancellationToken cancellationToken)
    {
        var currentAuthVersion = await _userRepository.GetAuthVersion(userId, cancellationToken);
        return currentAuthVersion.HasValue && currentAuthVersion.Value == tokenAuthVersion;
    }
}