using PRN232.Lab2.CoffeeStore.Models.Request.Order;
using PRN232.Lab2.CoffeeStore.Models.Response.Common;
using PRN232.Lab2.CoffeeStore.Models.Response.Order;

namespace PRN232.Lab2.CoffeeStore.Services.IService;

public interface IOrderService
{
    Task<PageResponse<OrderResponse>> GetAllOrdersAsync(OrderFilter filter);
    Task<OrderResponse?> GetOrderByIdAsync(int id);
    Task<OrderResponse> CreateOrderAsync(CreateOrderRequest request);
    Task UpdateOrderAsync(int id, UpdateOrderRequest request);
    Task DeleteOrderAsync(int id);
}