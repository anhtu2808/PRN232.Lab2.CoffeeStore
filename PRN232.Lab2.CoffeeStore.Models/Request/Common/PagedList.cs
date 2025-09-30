namespace PRN232.Lab2.CoffeeStore.Models.Request.Common;

public class PagedList<T>
{
    public List<T> Items { get; }
    public int Page { get; }
    public int PageSize { get; }
    public int TotalCount { get; }
    public bool HasPreviousPage => Page > 1;
    public bool HasNextPage => (Page * PageSize) < TotalCount;

    public PagedList(List<T> items, int totalCount, int page, int pageSize)
    {
        Items = items;
        TotalCount = totalCount;
        Page = page;
        PageSize = pageSize;
    }
}