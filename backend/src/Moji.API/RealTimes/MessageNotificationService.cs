using Microsoft.AspNetCore.SignalR;
using Moji.API.Hubs;
using Moji.BusinessLogic.Services.Messages;
using Moji.Contracts.Models.Conversations;
using Moji.Contracts.Models.Messages;

namespace Moji.API.RealTimes;

public class MessageNotificationService : IMessageNotificationService
{
    private readonly IHubContext<ChatHub, IChatClient> _chatHubContext;

    public MessageNotificationService(IHubContext<ChatHub, IChatClient> chatHubContext)
    {
        _chatHubContext = chatHubContext;
    }

    public async Task BroadcastMessageToConversationAsync(string conversationId, MessageResponse messageResponse)
    {
        await _chatHubContext.Clients.Group(conversationId).ReceiveMessage(messageResponse);
    }

    public async Task BroadcastMarkAsSeenToConversationAsync(string userId, string conversationId)
    {
        await _chatHubContext.Clients.User(userId).MarkAsSeen(userId, conversationId);
    }
}