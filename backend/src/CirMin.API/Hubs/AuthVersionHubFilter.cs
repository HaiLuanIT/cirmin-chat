using System.Security.Claims;
using Microsoft.AspNetCore.SignalR;
using CirMin.BusinessLogic.Services.Auth;

namespace CirMin.API.Hubs;

public class AuthVersionHubFilter : IHubFilter
{
    private readonly IAccessTokenValidator _accessTokenValidator;

    public AuthVersionHubFilter(IAccessTokenValidator accessTokenValidator)
    {
        _accessTokenValidator = accessTokenValidator;
    }

    public async ValueTask<object?> InvokeMethodAsync(HubInvocationContext invocationContext,
        Func<HubInvocationContext, ValueTask<object?>> next)
    {
        var userIdValue = invocationContext.Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var authVersionValue = invocationContext.Context.User?.FindFirst("auth_version")?.Value;
        if (!Guid.TryParse(userIdValue, out var userId) ||
            !int.TryParse(authVersionValue, out var tokenAuthVersion))
        {
            invocationContext.Context.Abort();
            throw new HubException("Invalid authentication claims");
        }

        var isValid =
            await _accessTokenValidator.IsValidAsync(userId, tokenAuthVersion,
                invocationContext.Context.ConnectionAborted);
        if (!isValid)
        {
            invocationContext.Context.Abort();
            throw new HubException("Session has been revoked");
        }

        return await next(invocationContext);
    }
}