namespace Moji.BusinessLogic.Models.CursorPagination;

public record CursorResponse<T>
{
    public List<T> Items { get; init; } = new();
    
    public string? NextCursor { get; init; }
    
    public bool HasMore { get; init; }
}