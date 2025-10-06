using System.Linq.Dynamic.Core;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using PRN232.Lab2.CoffeeStore.Models.Request.Common;
using PRN232.Lab2.CoffeeStore.Models.Response.Common;
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
        var entry = await _dbSet.AddAsync(entity);
        var navigationProperties = _context.Entry(entry.Entity).Navigations;
        foreach (var navigation in navigationProperties)
        {
            await navigation.LoadAsync();
        }
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

    public async Task<PageResponse<T>> GetPagedListAsync(
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
            foreach (var includeProperty in includeProperties
                         .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                query = query.Include(includeProperty.Trim());
            }
        }
        else if (!string.IsNullOrWhiteSpace(parameters.Keyword))
        {
            var search = parameters.Keyword.Trim().ToLower();

            var stringProperties = typeof(T)
                .GetProperties()
                .Where(p => p.PropertyType == typeof(string))
                .ToList();

            if (stringProperties.Any())
            {
                var parameter = Expression.Parameter(typeof(T), "e");
                Expression? orExpressions = null;

                foreach (var prop in stringProperties)
                {
                    var propAccess = Expression.Property(parameter, prop);

                    // Convert to lower
                    var toLowerCall = Expression.Call(propAccess, nameof(string.ToLower), Type.EmptyTypes);

                    // Constant for "%search%"
                    var searchPattern = Expression.Constant($"%{search}%");

                    // EF.Functions.Like(EF.Property<string>(e, "Prop"), "%search%")
                    var efFunctions = Expression.Property(null, typeof(EF), nameof(EF.Functions));
                    var likeMethod = typeof(DbFunctionsExtensions)
                        .GetMethod(nameof(DbFunctionsExtensions.Like),
                            new[] { typeof(DbFunctions), typeof(string), typeof(string) })!;

                    var likeCall = Expression.Call(
                        null,
                        likeMethod,
                        efFunctions,
                        toLowerCall,
                        searchPattern
                    );

                    // Combine with OR
                    orExpressions = orExpressions == null
                        ? likeCall
                        : Expression.OrElse(orExpressions, likeCall);
                }

                if (orExpressions != null)
                {
                    var lambda = Expression.Lambda<Func<T, bool>>(orExpressions, parameter);
                    query = query.Where(lambda);
                }
            }
        }

        // Sorting
        if (orderBy != null)
        {
            query = orderBy(query);
        }
        else if (!string.IsNullOrWhiteSpace(parameters.Sort))
        {
            query = query.OrderBy(parameters.Sort);
        }

        // Pagination
        var totalCount = await query.CountAsync();

        var items = await query
            .Skip((parameters.Page - 1) * parameters.PageSize)
            .Take(parameters.PageSize)
            .ToListAsync();

        return new PageResponse<T>(items, totalCount, parameters.Page, parameters.PageSize);
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