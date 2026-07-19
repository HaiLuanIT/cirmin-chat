using Moji.Contracts.Models.Messages;
using Moji.Contracts.Models.Messages.SendMessage;
using Moji.Contracts.Models.Paginations.CursorPagination;

namespace Moji.BusinessLogic.Services.Messages;

public interface IMessageService
{
    Task SendMessage(Guid senderId, SendMessageRequest request);

    Task<CursorPagingResult<MessageResponse>> GetConversationMessages(Guid currentUserId, Guid conversationId, int limit,
        string? cursor);

    Task<long?> MarkAsSeen(Guid currentUserId, Guid conversationId);
}