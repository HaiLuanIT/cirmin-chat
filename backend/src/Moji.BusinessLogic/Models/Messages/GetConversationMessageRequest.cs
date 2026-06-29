namespace Moji.BusinessLogic.Models;

public record GetConversationMessageRequest
{
    public string? NextCursor { get; init; }

    public int Limit { get; set; } = 20;
}