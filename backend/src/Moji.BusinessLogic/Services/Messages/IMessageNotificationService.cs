using Moji.BusinessLogic.Models;
using Moji.BusinessLogic.Models.Conversations;

namespace Moji.BusinessLogic.Services.Messages;

public interface IMessageNotificationService
{
    Task BroadcastMessageToConversationAsync(string conversationId, MessageResponse messageResponse,
        ConversationModel conversationResponse);
}