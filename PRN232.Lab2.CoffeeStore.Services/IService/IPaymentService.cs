using PRN232.Lab2.CoffeeStore.Models.Request.ZaloPay;

namespace PRN232.Lab2.CoffeeStore.Services.IService;

public interface IPaymentService
{
    Task<String> GetPaymentUrlAsync(int orderId);
    Task<bool> VerifyPaymentAsync(ZaloPayCallbackRequest request);
}