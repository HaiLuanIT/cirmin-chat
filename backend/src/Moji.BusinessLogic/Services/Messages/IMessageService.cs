using Moji.BusinessLogic.Models;
using Moji.BusinessLogic.Models.CursorPagination;

namespace Moji.BusinessLogic.Services.Messages;

public interface IMessageService
{
    Task<SendMessageResponse> SendMessage(Guid senderId, SendMessageRequest request);

    Task<CursorResponse<MessageResponse>> GetConversationMessages(Guid currentUserId, Guid conversationId, int limit,
        string? cursor);
}