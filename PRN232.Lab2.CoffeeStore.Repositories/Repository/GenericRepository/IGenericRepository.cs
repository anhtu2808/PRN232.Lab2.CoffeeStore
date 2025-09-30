using System.Linq.Expressions;
using PRN232.Lab2.CoffeeStore.Models.Request;
using PRN232.Lab2.CoffeeStore.Models.Request.Common;

namespace PRN232.Lab2.CoffeeStore.Repositories.Repository.GenericRepository;

public interface IGenericRepository<T> where T : class
{
    Task<T> GetByIdAsync(int id);

    Task<IEnumerable<T>> GetAllAsync();

    Task AddAsync(T entity);

    void Update(T entity);

    void Delete(T entity);

    Task<PagedList<T>> GetPagedListAsync(
        RequestParameters parameters,
        Expression<Func<T, bool>>? filter = null,
        Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
        string? includeProperties = ""
    );
}