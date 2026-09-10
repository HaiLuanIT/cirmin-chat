namespace CirMin.BusinessLogic.Services.Auth;

public interface ISessionNotificationService
{
    Task BroadcastClientLogoutAsync(string userId);
}