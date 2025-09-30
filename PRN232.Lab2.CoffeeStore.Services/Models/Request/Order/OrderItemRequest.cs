using System.ComponentModel.DataAnnotations;

namespace PRN232.Lab2.CoffeeStore.Services.Models.Request.Order;

public record OrderItemRequest(
    [Required]
    int ProductId,

    [Required]
    [Range(1, 100)]
    int Quantity
);