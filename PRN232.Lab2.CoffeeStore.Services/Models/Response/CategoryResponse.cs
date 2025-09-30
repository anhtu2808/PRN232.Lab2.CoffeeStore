namespace PRN232.Lab2.CoffeeStore.Services.Models.Response;

public record CategoryResponse(
    int CategoryId,
    string Name,
    string? Description,
    DateTime CreatedDate
);