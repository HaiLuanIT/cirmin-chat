using CirMin.API.Hubs;
using Microsoft.AspNetCore.SignalR;
using CirMin.BusinessLogic.Services.Auth;

namespace CirMin.API.RealTimes;

public class SessionNotificationService : ISessionNotificationService
{
    private readonly IHubContext<ChatHub, IChatClient> _chatHubContext;

    public SessionNotificationService(IHubContext<ChatHub, IChatClient> chatHubContext)
    {
        _chatHubContext = chatHubContext;
    }

    public async Task BroadcastClientLogoutAsync(string userId)
    {
        await _chatHubContext.Clients.User(userId).SessionRevoked();
    }
}