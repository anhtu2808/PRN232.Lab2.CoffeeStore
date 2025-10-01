namespace PRN232.Lab2.CoffeeStore.Models.Response.Product;

public record ProductResponse(
    int ProductId,
    string Name,
    string? Description,
    decimal Price,
    bool IsActive,
    int CategoryId,
    string CategoryName 
);