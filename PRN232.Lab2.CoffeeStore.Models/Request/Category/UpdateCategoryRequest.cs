using System.ComponentModel.DataAnnotations;

namespace PRN232.Lab2.CoffeeStore.Models.Request.Category;

public record UpdateCategoryRequest(
    
    [StringLength(100, MinimumLength = 3)]
    string Name,

    [StringLength(500)]
    string? Description
);