namespace PRN232.Lab2.CoffeeStore.Models.Response.Common;

public class PageResponse<T>
{
    public object Items { get; }
    public int Page { get; }
    public int PageSize { get; }
    public int TotalCount { get; }
    public bool HasPreviousPage => Page > 1;
    public bool HasNextPage => (Page * PageSize) < TotalCount;

    public PageResponse(object items, int totalCount, int page, int pageSize)
    {
        Items = items;
        TotalCount = totalCount;
        Page = page;
        PageSize = pageSize;
    }
    
}