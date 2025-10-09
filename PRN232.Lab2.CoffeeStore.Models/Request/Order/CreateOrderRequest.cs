using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace PRN232.Lab2.CoffeeStore.Models.Request.Order;

public class CreateOrderRequest
{
    [JsonIgnore]
    public string? UserId { get; set; }

    [Required]
    public string PaymentMethod { get; set; }

    [Required]
    [MinLength(1, ErrorMessage = "Order must have at least one item.")]
    public List<OrderItemRequest> OrderItems { get; set; } = new List<OrderItemRequest>();
}
