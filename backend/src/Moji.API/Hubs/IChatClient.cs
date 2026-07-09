using Moji.Contracts.Models.Conversations;
using Moji.Contracts.Models.Messages;

namespace Moji.API.Hubs;

public interface IChatClient
{
    Task ReceiveMessage(MessageResponse messageResponse);

    Task UserStatusChanged(string userId, bool isOnline);

    Task GetOnlineUsers(List<string> userIds);

    Task MarkAsSeen(string userId, string conversationId);
}