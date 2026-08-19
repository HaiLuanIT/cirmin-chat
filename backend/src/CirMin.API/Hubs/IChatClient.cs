using CirMin.Contracts.Models.Conversations;
using CirMin.Contracts.Models.Messages;

namespace CirMin.API.Hubs;

public interface IChatClient
{
    Task ReceiveMessage(MessageResponse messageResponse);

    Task UserStatusChanged(string userId, bool isOnline);

    Task GetOnlineUsers(List<string> userIds);

    Task UserSeenMessage(string userId, string conversationId, string lastMessageId);

    Task GroupConversationCreated(ConversationModel conversation);

    Task SessionRevoked();
}