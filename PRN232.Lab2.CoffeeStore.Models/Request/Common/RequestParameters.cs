namespace PRN232.Lab2.CoffeeStore.Models.Request.Common;

public class RequestParameters
{
    private const int MaxPageSize = 50;
    public string? Keyword { get; set; }
    public string? Sort { get; set; }
    public string? SortDirection { get; set; } = "asc";
    public List<string> IncludeProperties { get; set; } = new();
    public int Page { get; set; } = 1;

    private int _pageSize = 10;

    public int PageSize
    {
        get => _pageSize;
        set => _pageSize = (value > MaxPageSize) ? MaxPageSize : value;
    }
}