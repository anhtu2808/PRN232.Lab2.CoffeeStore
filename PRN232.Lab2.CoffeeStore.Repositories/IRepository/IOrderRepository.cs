using PRN232.Lab2.CoffeeStore.Models.Request.Order;
using PRN232.Lab2.CoffeeStore.Models.Response.Common;
using PRN232.Lab2.CoffeeStore.Repositories.Entity;

namespace PRN232.Lab2.CoffeeStore.Repositories.IRepository;

public interface IOrderRepository : IGenericRepository<Order, int>
{
    Task<PageResponse<Order>> GetAllOrders(OrderFilter filter);
}