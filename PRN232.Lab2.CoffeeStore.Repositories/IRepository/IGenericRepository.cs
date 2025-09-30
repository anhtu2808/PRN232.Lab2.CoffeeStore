using System.Linq.Expressions;
using PRN232.Lab2.CoffeeStore.Models.Request.Common;

namespace PRN232.Lab2.CoffeeStore.Repositories.IRepository;

public interface IGenericRepository<T, TKey> where T : class
{
    Task<T?> GetByIdAsync(TKey id);

    Task<IEnumerable<T>> GetAllAsync();

    Task AddAsync(T entity);

    // Đã chuyển sang async
    Task UpdateAsync(T entity);

    // Đã chuyển sang async
    Task DeleteAsync(T entity);

    Task<PagedList<T>> GetPagedListAsync(
        RequestParameters parameters,
        Expression<Func<T, bool>>? filter = null,
        Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
        string? includeProperties = ""
    );
}