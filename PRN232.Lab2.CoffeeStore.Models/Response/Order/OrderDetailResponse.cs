namespace PRN232.Lab2.CoffeeStore.Models.Response.Order;

public record OrderDetailResponse(
    int OrderDetailId,
    int ProductId,
    string ProductName, 
    int Quantity,
    decimal UnitPrice
);