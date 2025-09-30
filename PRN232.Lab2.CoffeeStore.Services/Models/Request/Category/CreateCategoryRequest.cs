using System.ComponentModel.DataAnnotations;

namespace PRN232.Lab2.CoffeeStore.Services.Models.Request.Category;

public record CreateCategoryRequest(
    [Required]
    [StringLength(100, MinimumLength = 3)]
    string Name,

    [StringLength(500)]
    string? Description
);