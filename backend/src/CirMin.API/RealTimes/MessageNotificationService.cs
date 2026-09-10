using CirMin.API.Hubs;
using Microsoft.AspNetCore.SignalR;
using CirMin.BusinessLogic.Services.Messages;
using CirMin.Contracts.Models.Conversations;
using CirMin.Contracts.Models.Messages;

namespace CirMin.API.RealTimes;

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
}