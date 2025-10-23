using System.ComponentModel.DataAnnotations;

namespace PRN232.Lab2.CoffeeStore.Models.Request.Product;

public record UpdateProductRequest(
    [StringLength(200, MinimumLength = 3)]
    string Name,

    string? Description,

    [Range(0.01, double.MaxValue)]
    decimal Price,

    int? CategoryId,
    
    bool IsActive
);