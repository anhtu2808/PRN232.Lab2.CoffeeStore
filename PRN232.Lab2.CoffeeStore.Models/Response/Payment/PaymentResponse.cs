namespace PRN232.Lab2.CoffeeStore.Models.Response;

public record PaymentResponse(
    int PaymentId,
    int OrderId,
    decimal Amount,
    DateTime PaymentDate,
    string PaymentMethod
);