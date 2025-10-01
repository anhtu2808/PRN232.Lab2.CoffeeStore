namespace PRN232.Lab2.CoffeeStore.Models.Response.Payment;

public record PaymentResponse(
    int PaymentId,
    int OrderId,
    decimal Amount,
    DateTime PaymentDate,
    string PaymentMethod
);