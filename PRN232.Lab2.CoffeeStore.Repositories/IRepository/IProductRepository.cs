using PRN232.Lab2.CoffeeStore.Models.Request.Common;
using PRN232.Lab2.CoffeeStore.Models.Request.Product;
using PRN232.Lab2.CoffeeStore.Models.Response.Common;
using PRN232.Lab2.CoffeeStore.Repositories.Entity;

namespace PRN232.Lab2.CoffeeStore.Repositories.IRepository;

public interface IProductRepository : IGenericRepository<Product, int>
{
    public Task<PageResponse<Product>> GetPagedProductsAsync(
        ProductFilter filter
        );
}