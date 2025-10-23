using System.ComponentModel.DataAnnotations;

namespace PRN232.Lab2.CoffeeStore.Models.Request.Common
{
    public class RequestParameters
    {
        private const int MaxPageSize = 50;
        public string? Keyword { get; set; }
        public string? Sort { get; set; }
        
        [RegularExpression("^(asc|desc)$", ErrorMessage = "SortDirection must be 'asc' or 'desc'")]
        public string? SortDirection { get; set; } = "asc";
        
        public List<string> IncludeProperties { get; set; } = new();

        [Range(1, int.MaxValue, ErrorMessage = "Page number must be greater than 0.")]
        public int Page { get; set; } = 1;

        private int _pageSize = 10;

        [Range(1, MaxPageSize, ErrorMessage = "Page size must be between 1 and {1}.")]
        public int PageSize
        {
            get => _pageSize;
            set => _pageSize = (value > MaxPageSize) ? MaxPageSize : value;
        }
    }
}