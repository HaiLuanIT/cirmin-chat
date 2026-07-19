namespace Moji.Contracts.Models.Paginations.CursorPagination;

public record CursorPagingResult<T>
{
    public List<T> Items { get; init; } = new();
    
    public string? NextCursor { get; init; }
    
    public bool HasMore { get; init; }
}