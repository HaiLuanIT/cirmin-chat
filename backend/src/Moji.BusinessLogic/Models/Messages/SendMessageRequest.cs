namespace Moji.BusinessLogic.Models;

public record SendMessageRequest(string Content, Guid ConversationId, string? ImgUrl);