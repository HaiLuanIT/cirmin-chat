using Moji.BusinessLogic.Models;
using Moji.BusinessLogic.Models.Conversations;
using Moji.BusinessLogic.Models.CursorPagination;

namespace Moji.BusinessLogic.Services.Messages;

public interface IMessageService
{
    Task SendMessage(Guid senderId, SendMessageRequest request);

    Task<CursorResponse<MessageResponse>> GetConversationMessages(Guid currentUserId, Guid conversationId, int limit,
        string? cursor);
}