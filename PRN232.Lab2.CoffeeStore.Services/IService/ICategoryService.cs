using PRN232.Lab2.CoffeeStore.Models.Request.Category;
using PRN232.Lab2.CoffeeStore.Models.Request.Common;
using PRN232.Lab2.CoffeeStore.Models.Response.Category;
using PRN232.Lab2.CoffeeStore.Models.Response.Common;

namespace PRN232.Lab2.CoffeeStore.Services.IService;

public interface ICategoryService
{
    Task<PageResponse<CategoryResponse>> GetAllAsync(RequestParameters parameters);
    Task<CategoryResponse?> GetByIdAsync(int id);
    Task<CategoryResponse> CreateAsync(CreateCategoryRequest request);
    Task<CategoryResponse> UpdateAsync(int id, UpdateCategoryRequest request);
    Task DeleteAsync(int id);
}