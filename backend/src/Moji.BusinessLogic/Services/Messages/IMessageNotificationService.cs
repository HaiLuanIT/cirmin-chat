using Moji.Contracts.Models.Conversations;
using Moji.Contracts.Models.Messages;

namespace Moji.BusinessLogic.Services.Messages;

public interface IMessageNotificationService
{
    Task BroadcastMessageToConversationAsync(string conversationId, MessageResponse messageResponse,
        ConversationModel conversationResponse);
}