namespace Moji.Contracts.Models.Paginations.OffsetPagination;

public record OffsetPagingResult<T>
{
    public List<T> Items { get; init; }
    
    public int PageNumber { get; init; }
    
    public int PageSize { get; init; }
    
    public int TotalCount { get; init; }
    
    public bool HasNextPage => PageNumber * PageSize < TotalCount;
}