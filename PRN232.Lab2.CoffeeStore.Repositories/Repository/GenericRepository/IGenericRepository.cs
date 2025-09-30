using System.Linq.Expressions;

namespace PRN232.Lab2.CoffeeStore.Repositories.Repository.GenericRepository;

public interface IGenericRepository<T, TId>
    where T : class
    where TId : notnull
{
    /// <summary>
    /// Gets an entity by its primary key.
    /// </summary>
    Task<T?> GetByIdAsync(TId id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a paged list of entities based on filters, sorting, and tracking options.
    /// </summary>
    Task<IEnumerable<T>> GetPagedAsync(
        int skip,
        int take,
        IEnumerable<Expression<Func<T, bool>>>? filters = null,
        Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
        bool asNoTracking = true,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Calculates the total number of pages for a given page size and filter set.
    /// </summary>
    Task<int> GetTotalPagesAsync(
        int pageSize,
        IEnumerable<Expression<Func<T, bool>>>? filters = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Returns the IQueryable to build custom queries upon.
    /// </summary>
    IQueryable<T> Query(bool asNoTracking = true);

    /// <summary>
    /// Adds a new entity.
    /// </summary>
    Task AddAsync(T entity, CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds a range of new entities.
    /// </summary>
    Task AddRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing entity.
    /// </summary>
    Task UpdateAsync(T entity, CancellationToken cancellationToken = default);

    /// <summary>
    /// Removes an entity.
    /// </summary>
    Task RemoveAsync(T entity, CancellationToken cancellationToken = default);

    /// <summary>
    /// Saves all changes made in this context to the database.
    /// </summary>
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}