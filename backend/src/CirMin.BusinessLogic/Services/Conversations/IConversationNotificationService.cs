using CirMin.Contracts.Models.Conversations;

namespace CirMin.BusinessLogic.Services.Conversations;

public interface IConversationNotificationService
{
    Task NotifyConversationCreatedAsync(ConversationModel conversationModel);
}