using PRN232.Lab2.CoffeeStore.Models.Request.Common;
using PRN232.Lab2.CoffeeStore.Models.Request.Product;
using PRN232.Lab2.CoffeeStore.Models.Response.Common;
using PRN232.Lab2.CoffeeStore.Models.Response.Product;

namespace PRN232.Lab2.CoffeeStore.Services.IService;

public interface IProductService
{
    Task<PageResponse<ProductResponse>> GetProductsAsync(ProductFilter filter);
    Task<ProductResponse?> GetProductByIdAsync(int id);
    Task<ProductResponse> CreateProductAsync(CreateProductRequest request);
    Task UpdateProductAsync(int id, UpdateProductRequest request);
    Task DeleteProductAsync(int id);
}