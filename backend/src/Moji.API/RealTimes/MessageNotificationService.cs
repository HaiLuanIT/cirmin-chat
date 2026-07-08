using Microsoft.AspNetCore.SignalR;
using Moji.API.Hubs;
using Moji.BusinessLogic.Models;
using Moji.BusinessLogic.Models.Conversations;
using Moji.BusinessLogic.Services.Messages;

namespace Moji.API.RealTimes;

public class MessageNotificationService : IMessageNotificationService
{
    private readonly IHubContext<ChatHub, IChatClient> _chatHubContext;

    public MessageNotificationService(IHubContext<ChatHub, IChatClient> chatHubContext)
    {
        _chatHubContext = chatHubContext;
    }

    public async Task BroadcastMessageToConversationAsync(string conversationId, MessageResponse messageResponse,
        ConversationModel conversationResponse)
    {
        await _chatHubContext.Clients.Group(conversationId).ReceiveMessage(messageResponse, conversationResponse);
    }
}