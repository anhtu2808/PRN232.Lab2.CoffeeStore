using PRN232.Lab2.CoffeeStore.Models.Request.Common;

namespace PRN232.Lab2.CoffeeStore.Models.Request.Product;

public class ProductFilter : RequestParameters
{
    public int? CategoryId { get; set; }


    public decimal? MinPrice { get; set; }


    public decimal? MaxPrice { get; set; }


    public bool? IsActive { get; set; }
}