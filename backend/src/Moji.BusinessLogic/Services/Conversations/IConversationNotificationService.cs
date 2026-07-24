using Moji.Contracts.Models.Conversations;

namespace Moji.BusinessLogic.Services.Conversations;

public interface IConversationNotificationService
{
    Task NotifyConversationCreatedAsync(ConversationModel conversationModel);
}