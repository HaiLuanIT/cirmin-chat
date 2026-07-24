using Microsoft.AspNetCore.SignalR;
using Moji.API.Hubs;
using Moji.BusinessLogic.Services.Conversations;
using Moji.Contracts.Models.Conversations;

namespace Moji.API.RealTimes;

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