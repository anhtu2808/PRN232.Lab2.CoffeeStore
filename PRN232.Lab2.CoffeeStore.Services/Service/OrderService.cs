using AutoMapper;
using PRN232.Lab2.CoffeeStore.Models.Enums;
using PRN232.Lab2.CoffeeStore.Models.Exception;
using PRN232.Lab2.CoffeeStore.Models.Request.Order;
using PRN232.Lab2.CoffeeStore.Models.Response.Common;
using PRN232.Lab2.CoffeeStore.Models.Response.Order;
using PRN232.Lab2.CoffeeStore.Models.Response.Product;
using PRN232.Lab2.CoffeeStore.Repositories.Entity;
using PRN232.Lab2.CoffeeStore.Repositories.UnitOfWork;
using PRN232.Lab2.CoffeeStore.Services.IService;

namespace PRN232.Lab2.CoffeeStore.Services.Service;

public class OrderService : IOrderService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public OrderService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<PageResponse<OrderResponse>> GetAllOrdersAsync(OrderFilter filter)
    {
        var orderPages = await _unitOfWork.Orders.GetAllOrders(filter);
        var productResponses = _mapper.Map<List<OrderResponse>>(orderPages.Items);
        return new PageResponse<OrderResponse>(
            productResponses,
            orderPages.TotalCount,
            orderPages.Page,
            orderPages.PageSize
        );
    }

    public async Task<OrderResponse?> GetOrderByIdAsync(int id)
    {
        throw new NotImplementedException();
    }

    public async Task<OrderResponse> CreateOrderAsync(CreateOrderRequest request)
    {
        var order = _mapper.Map<Order>(request);
        order.Status = nameof(OrderStatus.Pending);
        order.OrderDetails = new List<OrderDetail>();
        foreach (var item in request.OrderItems)
        {
            var product = await _unitOfWork.Products.GetByIdAsync(item.ProductId);
            if (product == null)
            {
                throw new AppException(ErrorCode.ProductNotFound);
            }

            var orderDetail = _mapper.Map<OrderDetail>(item);
            order.OrderDetails.Add(orderDetail);
        }

        await _unitOfWork.Orders.AddAsync(order);
        await _unitOfWork.CompleteAsync();
        return _mapper.Map<OrderResponse>(order);
    }

    public async Task UpdateOrderAsync(int id, UpdateOrderRequest request)
    {
        throw new NotImplementedException();
    }

    public async Task DeleteOrderAsync(int id)
    {
        throw new NotImplementedException();
    }
}