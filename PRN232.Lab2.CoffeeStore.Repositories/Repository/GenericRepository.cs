using System.Linq.Dynamic.Core;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using PRN232.Lab2.CoffeeStore.Models.Request.Common;
using PRN232.Lab2.CoffeeStore.Repositories.Entity;
using PRN232.Lab2.CoffeeStore.Repositories.IRepository;

namespace PRN232.Lab2.CoffeeStore.Repositories.Repository;

public class GenericRepository<T, TKey> : IGenericRepository<T, TKey> where T : class
{
    private readonly CoffeeStoreDbContext _context;
    private readonly DbSet<T> _dbSet;
    private IGenericRepository<T, TKey> _genericRepositoryImplementation;

    public GenericRepository(CoffeeStoreDbContext context)
    {
        _context = context;
        _dbSet = context.Set<T>();
    }

    public async Task<T?> GetByIdAsync(TKey id)
    {
        return await _dbSet.FindAsync(id);
    }

    public async Task<IEnumerable<T>> GetAllAsync()
    {
        return await _dbSet.ToListAsync();
    }

    public async Task AddAsync(T entity)
    {
        await _dbSet.AddAsync(entity);
    }

    public Task UpdateAsync(T entity)
    {
        _dbSet.Attach(entity);
        _context.Entry(entity).State = EntityState.Modified;
        return Task.CompletedTask;
    }


    public Task DeleteAsync(T entity)
    {
        _dbSet.Remove(entity);
        return Task.CompletedTask;
    }

    public async Task<PagedList<T>> GetPagedListAsync(
        RequestParameters parameters,
        Expression<Func<T, bool>>? filter = null,
        Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
        string? includeProperties = "")
    {
        IQueryable<T> query = _dbSet;

        if (filter != null)
        {
            query = query.Where(filter);
        }

        if (!string.IsNullOrWhiteSpace(includeProperties))
        {
            foreach (var includeProperty in includeProperties.Split
                         (new[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                query = query.Include(includeProperty.Trim());
            }
        }

        if (orderBy != null)
        {
            query = orderBy(query);
        }
        else if (!string.IsNullOrWhiteSpace(parameters.Sort))
        {
            query = query.OrderBy(parameters.Sort);
        }

        var totalCount = await query.CountAsync();

        var items = await query
            .Skip((parameters.Page - 1) * parameters.PageSize)
            .Take(parameters.PageSize)
            .ToListAsync();

        return new PagedList<T>(items, totalCount, parameters.Page, parameters.PageSize);
    }

    public async Task<T?> GetFirstOrDefaultAsync(
        Expression<Func<T, bool>> filter,
        string? includeProperties = "")
    {
        IQueryable<T> query = _dbSet;

        if (!string.IsNullOrWhiteSpace(includeProperties))
        {
            foreach (var includeProperty in includeProperties.Split
                         (new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                query = query.Include(includeProperty.Trim());
            }
        }

        // Áp dụng bộ lọc và trả về phần tử đầu tiên hoặc null
        return await query.FirstOrDefaultAsync(filter);
    }
}