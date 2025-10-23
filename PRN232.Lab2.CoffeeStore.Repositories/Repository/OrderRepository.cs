using Microsoft.EntityFrameworkCore;
using PRN232.Lab2.CoffeeStore.Models.Enums;
using PRN232.Lab2.CoffeeStore.Models.Request.Order;
using PRN232.Lab2.CoffeeStore.Models.Response.Common;
using PRN232.Lab2.CoffeeStore.Repositories.Entity;
using PRN232.Lab2.CoffeeStore.Repositories.IRepository;

namespace PRN232.Lab2.CoffeeStore.Repositories.Repository;

public class OrderRepository : GenericRepository<Order, int>, IOrderRepository
{
    private readonly DbSet<Order> _dbSet;

    public OrderRepository(CoffeeStoreDbContext context) : base(context)
    {
        _dbSet = context.Set<Order>();
    }

    public async Task<PageResponse<Order>> GetAllOrders(OrderFilter filter)
    {
        var query = _dbSet
            .Include(o => o.OrderDetails)
            .ThenInclude(od => od.Product)
            .AsQueryable();

        if (filter.UserId.HasValue)
        {
            query = query.Where(o => o.UserId == filter.UserId.Value);
        }

        if (filter.Status != null)
        {
            string statusName = ((OrderStatus)filter.Status.Value).ToString();
            query = query.Where(o => o.Status == statusName);
        }

        if (filter.FromDate.HasValue)
        {
            query = query.Where(o => o.OrderDate >= filter.FromDate.Value);
        }

        if (filter.ToDate.HasValue)
        {
            query = query.Where(o => o.OrderDate <= filter.ToDate.Value);
        }

        int totalCount = query.Count();

        // Paging
        var orders = query
            .OrderByDescending(o => o.OrderDate)
            .Skip((filter.Page - 1) * filter.PageSize)
            .Take(filter.PageSize)
            .ToList();

        return new PageResponse<Order>(orders, totalCount, filter.Page, filter.PageSize);
    }
}