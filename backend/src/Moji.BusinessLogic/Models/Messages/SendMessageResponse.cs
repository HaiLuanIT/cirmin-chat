namespace Moji.BusinessLogic.Models;

public record SendMessageResponse(long MessageId, Guid SenderId, Guid ConversationId, string Message, DateTimeOffset SentAt);