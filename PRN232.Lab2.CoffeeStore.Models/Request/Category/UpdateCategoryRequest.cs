using System.ComponentModel.DataAnnotations;

namespace PRN232.Lab2.CoffeeStore.Models.Request.Category;

public record CreateProductRequest(
    [Required]
    [StringLength(200, MinimumLength = 3)]
    string Name,
    string? Description,
    [Required]
    [Range(0.01, double.MaxValue)]
    decimal Price,
    [Required] int CategoryId
);