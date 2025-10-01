namespace PRN232.Lab2.CoffeeStore.Models.Response.Category;

public record CategoryResponse(
    int CategoryId,
    string Name,
    string? Description,
    DateTime CreatedDate
);