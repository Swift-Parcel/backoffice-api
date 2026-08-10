namespace SwiftParcel.Application.Common.Models;

public class PagedList<T>
{
    public IReadOnlyList<T> Items { get; init; }
    public int PageNumber { get; init; }
    public int PageSize { get; init; }
    public int TotalCount { get; init; }
    public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
    public bool HasPreviousPage => PageNumber > 1;
    public bool HasNextPage => PageNumber < TotalPages;
    
    public PagedList(IReadOnlyList<T> items, int count, int pageNumber, int pageSize)
    {
        Items = items;
        TotalCount = count;
        PageNumber = pageNumber < 1 ? 1 : pageNumber;
        PageSize = pageSize;
    }
}