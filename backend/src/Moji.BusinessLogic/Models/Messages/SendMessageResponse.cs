namespace Moji.BusinessLogic.Models;

public record SendMessageResponse(long Id, Guid SenderId, Guid ConversationId, string Message, DateTimeOffset SentAt);