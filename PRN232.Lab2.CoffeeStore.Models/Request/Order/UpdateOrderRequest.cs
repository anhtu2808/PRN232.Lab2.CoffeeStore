using System.ComponentModel.DataAnnotations;

namespace PRN232.Lab2.CoffeeStore.Models.Request.Order;

public record UpdateOrderRequest(
    [Required] string Status
);