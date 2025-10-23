using System.Linq.Expressions;
using PRN232.Lab2.CoffeeStore.Models.Request.Common;
using PRN232.Lab2.CoffeeStore.Models.Response.Common;
using PRN232.Lab2.CoffeeStore.Repositories.Entity;
using PRN232.Lab2.CoffeeStore.Repositories.IRepository;

namespace CoffeeStore.Tests.Repository;

/// <summary>
/// Fake CategoryRepository - Simple implementation for testing
/// Uses Dictionary as in-memory storage instead of real database
/// </summary>
public class FakeCategoryRepository : ICategoryRepository
{
    // In-memory "database" using Dictionary
    private readonly Dictionary<int, Category> _categories;

    // Constructor: Initialize with some test data
    public FakeCategoryRepository()
    {
        _categories = new Dictionary<int, Category>
        {
            { 1, new Category { CategoryId = 1, Name = "Coffee", Description = "Hot coffee drinks" } },
            { 2, new Category { CategoryId = 2, Name = "Tea", Description = "Various tea types" } },
            { 3, new Category { CategoryId = 3, Name = "Juice", Description = "Fresh juices" } }
        };
    }

    // GET by ID - Returns category if exists, null otherwise
    public async Task<Category?> GetByIdAsync(int id)
    {
        await Task.CompletedTask; // Simulate async operation
        
        // Check if category exists in fake database
        if (_categories.ContainsKey(id))
        {
            return _categories[id];
        }
        
        return null; // Category not found
    }

    public async Task<IEnumerable<Category>> GetAllAsync()
    {
        throw new NotImplementedException();
    }

    public async Task AddAsync(Category entity)
    {
        throw new NotImplementedException();
    }


    // Not implemented - throw exception if called
    public Task SaveAsync(Category category)
    {
        throw new NotImplementedException("SaveAsync not implemented in FakeCategoryRepository");
    }

    public Task UpdateAsync(Category category)
    {
        throw new NotImplementedException("UpdateAsync not implemented in FakeCategoryRepository");
    }

    public async Task DeleteAsync(Category entity)
    {
        throw new NotImplementedException();
    }

    public async Task<PageResponse<Category>> GetPagedListAsync(RequestParameters parameters, Expression<Func<Category, bool>>? filter = null, Func<IQueryable<Category>, IOrderedQueryable<Category>>? orderBy = null,
        string? includeProperties = "")
    {
        throw new NotImplementedException();
    }

    public async Task<Category?> GetFirstOrDefaultAsync(Expression<Func<Category, bool>> filter, string? includeProperties = "")
    {
        throw new NotImplementedException();
    }

    public Task DeleteAsync(int id)
    {
        throw new NotImplementedException("DeleteAsync not implemented in FakeCategoryRepository");
    }
}