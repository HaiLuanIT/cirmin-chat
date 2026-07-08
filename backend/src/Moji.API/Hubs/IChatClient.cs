using Moji.BusinessLogic.Models;
using Moji.BusinessLogic.Models.Conversations;

namespace Moji.API.Hubs;

public interface IChatClient
{
    Task ReceiveMessage(MessageResponse messageResponse, ConversationModel conversationResponse);

    Task UserStatusChanged(string userId, bool isOnline);

    Task GetOnlineUsers(List<string> userIds);
}