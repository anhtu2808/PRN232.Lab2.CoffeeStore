namespace PRN232.Lab2.CoffeeStore.Models.Response;

public record OrderResponse(
    int OrderId,
    string UserId,
    DateTime OrderDate,
    string Status,
    decimal TotalAmount, 
    PaymentResponse? Payment,
    List<OrderDetailResponse> OrderDetails
);