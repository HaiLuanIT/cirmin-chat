using CirMin.API.Hubs;
using Microsoft.AspNetCore.SignalR;
using CirMin.BusinessLogic.Services.Conversations;
using CirMin.Contracts.Models.Conversations;

namespace CirMin.API.RealTimes;

public class ConversationNotificationService : IConversationNotificationService
{
    private readonly IHubContext<ChatHub, IChatClient> _chatHubContext;

    public ConversationNotificationService(IHubContext<ChatHub, IChatClient> chatHubContext)
    {
        _chatHubContext = chatHubContext;
    }

    public async Task NotifyConversationCreatedAsync(ConversationModel conversationModel)
    {
        if (conversationModel.Members.Any())
            foreach (var member in conversationModel.Members)
                await _chatHubContext.Clients.User(member.UserId.ToString())
                    .GroupConversationCreated(conversationModel);
    }
}