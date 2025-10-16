using System.Linq.Dynamic.Core;
using System.Linq.Expressions;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using PRN232.Lab2.CoffeeStore.Models.Request.User;
using PRN232.Lab2.CoffeeStore.Models.Response.Common;
using PRN232.Lab2.CoffeeStore.Repositories.Entity;
using PRN232.Lab2.CoffeeStore.Repositories.IRepository;

namespace PRN232.Lab2.CoffeeStore.Repositories.Repository;

public class UserRepository : GenericRepository<User, Guid>, IUserRepository
{
    private readonly DbSet<User> _dbSet;

    public UserRepository(CoffeeStoreDbContext context) : base(context)
    {
        _dbSet = context.Set<User>();
    }

    public async Task<User?> GetByUsernameAndPasswordAsync(string username, string password)
    {
        return await _dbSet.FirstOrDefaultAsync(u => u.Username == username && u.PasswordHash == password);
    }

    public async Task<PageResponse<User>> GetPagedUsersAsync(UserFilterRequest filter)
    {
        IQueryable<User> query = _dbSet;

        // Apply keyword search (từ RequestParameters base class)
        if (!string.IsNullOrWhiteSpace(filter.Keyword))
        {
            var searchTerm = filter.Keyword.Trim().ToLower();
            query = query.Where(u => u.Username.ToLower().Contains(searchTerm) ||
                                     u.Email.ToLower().Contains(searchTerm));
        }

        // Apply specific username filter
        if (!string.IsNullOrEmpty(filter.Username))
        {
            query = query.Where(u => u.Username.ToLower().Contains(filter.Username.ToLower()));
        }

        // Apply specific email filter  
        if (!string.IsNullOrEmpty(filter.Email))
        {
            query = query.Where(u => u.Email.ToLower().Contains(filter.Email.ToLower()));
        }

        // Apply date range filters
        if (filter.CreatedDateFrom.HasValue)
        {
            query = query.Where(u => u.CreatedDate >= filter.CreatedDateFrom.Value);
        }

        if (filter.CreatedDateTo.HasValue)
        {
            query = query.Where(u => u.CreatedDate <= filter.CreatedDateTo.Value);
        }

        var isDescending = filter.SortDirection == "desc";

        query = ApplyOrderByPropertyName(query, filter.Sort, isDescending);

        // Get total count before pagination
        var totalCount = await query.CountAsync();

        // Apply pagination
        query = query
            .Skip((filter.Page - 1) * filter.PageSize)
            .Take(filter.PageSize);

        //  Field selection
        if (filter.IncludeProperties.Any())
        {
            var selectString = "new (" + string.Join(",", filter.IncludeProperties) + ")";
            var projected = await query.Select(selectString).ToDynamicListAsync();
            return new PageResponse<User>(projected, totalCount, filter.Page, filter.PageSize);
        }
        // Default full entity
        var items = await query.ToListAsync();
        return new PageResponse<User>(items, totalCount, filter.Page, filter.PageSize);
    }

    private static IDictionary<string, object?> ShapeData<T>(T source, IEnumerable<string> fields)
    {
        var expando = new System.Dynamic.ExpandoObject() as IDictionary<string, object?>;
        var props = typeof(T).GetProperties(System.Reflection.BindingFlags.Public |
                                            System.Reflection.BindingFlags.Instance);

        foreach (var field in fields)
        {
            var prop = props.FirstOrDefault(p => p.Name.Equals(field, StringComparison.OrdinalIgnoreCase));
            if (prop != null)
            {
                expando[prop.Name] = prop.GetValue(source);
            }
        }

        return expando;
    }
}