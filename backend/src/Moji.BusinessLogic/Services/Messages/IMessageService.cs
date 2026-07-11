using Moji.Contracts.Models.CursorPagination;
using Moji.Contracts.Models.Messages;
using Moji.Contracts.Models.Messages.SendMessage;

namespace Moji.BusinessLogic.Services.Messages;

public interface IMessageService
{
    Task SendMessage(Guid senderId, SendMessageRequest request);

    Task<CursorResponse<MessageResponse>> GetConversationMessages(Guid currentUserId, Guid conversationId, int limit,
        string? cursor);

    Task<long?> MarkAsSeen(Guid currentUserId, Guid conversationId);
}