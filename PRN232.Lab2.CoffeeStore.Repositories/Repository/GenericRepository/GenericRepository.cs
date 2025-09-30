using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using PRN232.Lab2.CoffeeStore.Repositories.Entity;

namespace PRN232.Lab2.CoffeeStore.Repositories.Repository.GenericRepository;

public class GenericRepository<T, TId> : IGenericRepository<T, TId>
    where T : class
    where TId : notnull
{
    private readonly CoffeeStoreDbContext _dbContext;
    private readonly DbSet<T> _dbSet;

    public GenericRepository(CoffeeStoreDbContext dbContext)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        _dbSet = _dbContext.Set<T>();
    }

    public async Task<T?> GetByIdAsync(TId id, CancellationToken cancellationToken = default)
    {
        // Using FindAsync is optimized for finding by primary key
        return await _dbSet.FindAsync([id], cancellationToken);
    }

    

    public virtual async Task<IEnumerable<T>> GetPagedAsync(
        int skip,
        int take,
        IEnumerable<Expression<Func<T, bool>>>? filters = null,
        Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
        bool asNoTracking = true,
        CancellationToken cancellationToken = default)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(skip);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(take);

        IQueryable<T> query = _dbSet;

        // Apply all filters using AND logic
        if (filters is not null)
        {
            foreach (var predicate in filters)
            {
                if (predicate is null)
                    throw new ArgumentNullException(nameof(filters), "One of the filter expressions is null.");
                query = query.Where(predicate);
            }
        }

        // Apply sorting, or a default order to ensure consistent paging
        query = orderBy != null ? orderBy(query) : query.OrderBy(_ => true);

        // Apply paging
        query = query.Skip(skip).Take(take);
        
        // Execute query
        return asNoTracking
            ? await query.AsNoTracking().ToListAsync(cancellationToken)
            : await query.ToListAsync(cancellationToken);
    }

    private async Task<int> CountAsync(
        IEnumerable<Expression<Func<T, bool>>>? filters = null,
        CancellationToken cancellationToken = default)
    {
        IQueryable<T> query = _dbSet;

        if (filters is not null)
        {
            foreach (var predicate in filters)
            {
                if (predicate is null)
                    throw new ArgumentNullException(nameof(filters), "One of the filter expressions is null.");
                query = query.Where(predicate);
            }
        }

        return await query.CountAsync(cancellationToken);
    }

    public virtual async Task<int> GetTotalPagesAsync(
        int pageSize,
        IEnumerable<Expression<Func<T, bool>>>? filters = null,
        CancellationToken cancellationToken = default)
    {
        if (pageSize <= 0)
            throw new ArgumentOutOfRangeException(nameof(pageSize), "pageSize must be greater than zero.");

        var totalCount = await CountAsync(filters, cancellationToken);
        if (totalCount == 0) return 0;
        return (int)Math.Ceiling(totalCount / (double)pageSize);
    }

    public virtual IQueryable<T> Query(bool asNoTracking = true)
        => asNoTracking ? _dbSet.AsNoTracking() : _dbSet;

    public virtual async Task AddAsync(T entity, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(entity);
        await _dbSet.AddAsync(entity, cancellationToken);
    }

    public virtual async Task AddRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(entities);
        await _dbSet.AddRangeAsync(entities, cancellationToken);
    }

    public virtual Task UpdateAsync(T entity, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(entity);
        _dbSet.Update(entity);
        return Task.CompletedTask;
    }

    public virtual Task RemoveAsync(T entity, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(entity);
        _dbSet.Remove(entity);
        return Task.CompletedTask;
    }

    public virtual Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        => _dbContext.SaveChangesAsync(cancellationToken);
}