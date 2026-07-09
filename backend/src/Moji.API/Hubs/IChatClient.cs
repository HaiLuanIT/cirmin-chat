using Moji.Contracts.Models.Conversations;
using Moji.Contracts.Models.Messages;

namespace Moji.API.Hubs;

public interface IChatClient
{
    Task ReceiveMessage(MessageResponse messageResponse, ConversationModel conversationResponse);

    Task UserStatusChanged(string userId, bool isOnline);

    Task GetOnlineUsers(List<string> userIds);
}