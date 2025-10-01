using PRN232.Lab2.CoffeeStore.Models.Response.Payment;

namespace PRN232.Lab2.CoffeeStore.Models.Response.Order;

public record OrderResponse(
    int OrderId,
    string UserId,
    DateTime OrderDate,
    string Status,
    decimal TotalAmount, 
    PaymentResponse? Payment,
    List<OrderDetailResponse> OrderDetails
);