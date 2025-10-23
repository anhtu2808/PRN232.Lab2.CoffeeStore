using PRN232.Lab2.CoffeeStore.Models.Request.Common;
using PRN232.Lab2.CoffeeStore.Models.Request.Payment;
using PRN232.Lab2.CoffeeStore.Models.Request.ZaloPay;
using PRN232.Lab2.CoffeeStore.Models.Response.Common;
using PRN232.Lab2.CoffeeStore.Models.Response.Payment;

namespace PRN232.Lab2.CoffeeStore.Services.IService;

public interface IPaymentService
{
    Task<String> GetPaymentUrlAsync(int orderId);

    Task<PageResponse<PaymentResponse>> GetPaymentsAsync(PaymentFilter filter);
    Task<bool> VerifyPaymentAsync(ZaloPayCallbackRequest request);
}