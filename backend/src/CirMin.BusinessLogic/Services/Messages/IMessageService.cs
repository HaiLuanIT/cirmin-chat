using CirMin.Contracts.Models.Messages;
using CirMin.Contracts.Models.Messages.SendMessage;
using CirMin.Contracts.Models.Paginations.CursorPagination;

namespace CirMin.BusinessLogic.Services.Messages;

public interface IMessageService
{
    Task SendMessage(Guid senderId, SendMessageRequest request);

    Task<CursorPagingResult<MessageResponse>> GetConversationMessages(Guid currentUserId, Guid conversationId, int limit,
        string? cursor);

    Task<long?> MarkAsSeen(Guid currentUserId, Guid conversationId);
}