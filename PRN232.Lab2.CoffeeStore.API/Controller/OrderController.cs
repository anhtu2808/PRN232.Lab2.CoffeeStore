using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PRN232.Lab2.CoffeeStore.API.Utils;
using PRN232.Lab2.CoffeeStore.Models.Request.Order;
using PRN232.Lab2.CoffeeStore.Models.Response.Order;
using PRN232.Lab2.CoffeeStore.Models.Response.Common;
using PRN232.Lab2.CoffeeStore.Services.IService;

namespace PRN232.Lab2.CoffeeStore.API.Controller;

[ApiController]
[Authorize]
[Route("orders")]
public class OrderController : ControllerBase
{
    private readonly IOrderService _orderService;
    private readonly JwtUtils _jwtUtils;

    public OrderController(IOrderService orderService, JwtUtils jwtUtils)
    {
        _orderService = orderService;
        _jwtUtils = jwtUtils;
    }


    [HttpGet]
    public async Task<IActionResult> GetAllOrders([FromQuery] OrderFilter filter)
    {
        var orders = await _orderService.GetAllOrdersAsync(filter);
        var response = new ApiResponse<PageResponse<OrderResponse>>
        {
            StatusCode = 200,
            Message = "Orders retrieved successfully.",
            Data = orders
        };
        return Ok(response);
    }


    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetOrderById(int id)
    {
        var order = await _orderService.GetOrderByIdAsync(id);
        if (order == null)
        {
            return NotFound(new ApiResponse<object>
            {
                StatusCode = 404,
                Message = "Order not found."
            });
        }

        var response = new ApiResponse<OrderResponse>
        {
            StatusCode = 200,
            Message = "Order retrieved successfully.",
            Data = order
        };
        return Ok(response);
    }


    [HttpPost]
    public async Task<IActionResult> CreateOrder([FromBody] CreateOrderRequest request)
    {
        var userId = _jwtUtils.GetUserIdFromToken();
        request.UserId = userId;
        var newOrder = await _orderService.CreateOrderAsync(request);
        var response = new ApiResponse<OrderResponse>
        {
            StatusCode = 201,
            Message = "Order created successfully.",
            Data = newOrder
        };
        return CreatedAtAction(nameof(GetOrderById), new { id = newOrder.OrderId }, response);
    }


    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateOrder(int id, [FromBody] UpdateOrderRequest request)
    {
       var order =  await _orderService.UpdateOrderAsync(id, request);
        return Ok(new ApiResponse<object>
        {
            StatusCode = 200,
            Message = "Order updated successfully.",
            Data = order
        });
    }

    [HttpDelete("{id:int}")]
    [Authorize]
    public async Task<IActionResult> DeleteOrder(int id)
    {
        await _orderService.DeleteOrderAsync(id);
        return NoContent();
    }
}