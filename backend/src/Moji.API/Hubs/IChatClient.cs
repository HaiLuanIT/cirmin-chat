namespace Moji.API.Hubs;

public interface IChatClient
{
    Task ReceiveMessage(string conversationId, string message);

    Task UserStatusChanged(string userId, bool isOnline);

    Task GetOnlineUsers(List<string> userIds);
}