using System.Linq.Dynamic.Core;
using Microsoft.EntityFrameworkCore;
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
        ProductFilter filter
    )
    {
        IQueryable<Product> query = _dbSet.Include(p => p.Category);
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

        var isDescending = filter.SortDirection == "desc";
        query = ApplyOrderByPropertyName(query, filter.Sort, isDescending);
        var totalCount = await query.CountAsync();

        var page = filter.Page > 0 ? filter.Page : 1;
        var pageSize = filter.PageSize > 0 ? filter.PageSize : 10;

        query = query
            .Skip((page - 1) * pageSize)
            .Take(pageSize);
        //  Field selection
        if (filter.IncludeProperties.Any())
        {
            var selectString = "new (" + string.Join(",", filter.IncludeProperties) + ")";
            var projected = await query.Select(selectString).ToDynamicListAsync();
            return new PageResponse<Product>(projected, totalCount, filter.Page, filter.PageSize);
        }

        // Default full entity
        var items = await query.ToListAsync();
        return new PageResponse<Product>(items, totalCount, filter.Page, filter.PageSize);
    }

    public new Task DeleteAsync(Product entity)
    {
        entity.IsActive = false;
        return base.UpdateAsync(entity);
    }
}