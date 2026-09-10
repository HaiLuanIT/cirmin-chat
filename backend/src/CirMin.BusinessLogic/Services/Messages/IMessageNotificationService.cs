using CirMin.Contracts.Models.Conversations;
using CirMin.Contracts.Models.Messages;

namespace CirMin.BusinessLogic.Services.Messages;

public interface IMessageNotificationService
{
    Task BroadcastMessageToConversationAsync(string conversationId, MessageResponse messageResponse);
}