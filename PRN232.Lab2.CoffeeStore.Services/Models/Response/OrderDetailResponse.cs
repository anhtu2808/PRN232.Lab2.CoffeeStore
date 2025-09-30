namespace PRN232.Lab2.CoffeeStore.Services.Models.Response;

public record OrderDetailResponse(
    int OrderDetailId,
    int ProductId,
    string ProductName, 
    int Quantity,
    decimal UnitPrice
);