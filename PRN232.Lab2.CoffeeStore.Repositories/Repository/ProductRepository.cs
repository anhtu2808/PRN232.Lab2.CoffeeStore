using Microsoft.EntityFrameworkCore;
using PRN232.Lab2.CoffeeStore.Models.Request.Common;
using PRN232.Lab2.CoffeeStore.Models.Request.Product;
using PRN232.Lab2.CoffeeStore.Models.Response.Common;
using PRN232.Lab2.CoffeeStore.Repositories.Entity;
using PRN232.Lab2.CoffeeStore.Repositories.IRepository;

namespace PRN232.Lab2.CoffeeStore.Repositories.Repository;

public class ProductRepository : GenericRepository<Product, int>, IProductRepository
{
    private readonly DbSet<Product> _dbSet;

    public ProductRepository(CoffeeStoreDbContext context) : base(context)
    {
        _dbSet = context.Set<Product>();
    }

    public async Task<PageResponse<Product>> GetPagedProductsAsync(
        ProductFilter filter,
        Func<IQueryable<Product>, IOrderedQueryable<Product>>? orderBy = null,
        string includeProperties = "")
    {
        IQueryable<Product> query = _dbSet;
        if (!string.IsNullOrWhiteSpace(filter.Keyword))
        {
            var searchTerm = filter.Keyword.Trim().ToLower();
            query = query.Where(p => p.Name.ToLower().Contains(searchTerm) ||
                                     (p.Description != null && p.Description.ToLower().Contains(searchTerm)));
        }

        if (filter.CategoryId is > 0)
        {
            query = query.Where(p => p.CategoryId == filter.CategoryId.Value);
        }

        if (filter.MinPrice.HasValue)
        {
            query = query.Where(p => p.Price >= filter.MinPrice.Value);
        }

        if (filter.MaxPrice.HasValue)
        {
            query = query.Where(p => p.Price <= filter.MaxPrice.Value);
        }

        if (filter.IsActive.HasValue)
        {
            query = query.Where(p => p.IsActive == filter.IsActive.Value);
        }

        // foreach (var includeProperty in includeProperties.Split
        //              (new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
        // {
        //     query = query.Include(includeProperty);
        // }

        if (orderBy != null)
        {
            query = orderBy(query);
        }
        else
        {
            query = query.OrderBy(p => p.ProductId);
        }

        var totalCount = await query.CountAsync();

        var items = await query
            .Skip((filter.Page - 1) * filter.PageSize)
            .Take(filter.PageSize)
            .ToListAsync();

        return new PageResponse<Product>(items, totalCount, filter.Page, filter.PageSize);
    }

    public new Task DeleteAsync(Product entity)
    {
        entity.IsActive = false;
        return base.UpdateAsync(entity);
    }
}