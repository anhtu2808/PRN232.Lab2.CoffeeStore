using System.ComponentModel.DataAnnotations;

namespace PRN232.Lab2.CoffeeStore.Models.Request.Category;

public record CreateCategoryRequest
{
    [Required(AllowEmptyStrings = false, ErrorMessage = "Name is required.")]
    [StringLength(100, MinimumLength = 3, ErrorMessage = "Name must be between 3 and 100 characters.")]
    public string Name { get; init; } = string.Empty;

    [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters.")]
    public string? Description { get; init; }
}