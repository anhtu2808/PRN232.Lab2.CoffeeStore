using System.ComponentModel.DataAnnotations;

namespace PRN232.Lab2.CoffeeStore.Models.Request.Order;

public record CreateOrderRequest(
    [Required]
    string UserId,

    [Required]
    string PaymentMethod,

    [Required]
    [MinLength(1, ErrorMessage = "Order must have at least one item.")]
    List<OrderItemRequest> OrderItems
);