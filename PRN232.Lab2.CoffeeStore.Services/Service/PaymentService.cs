using System.Net.Http.Json;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Options;
using PRN232.Lab2.CoffeeStore.Models.Enums;
using PRN232.Lab2.CoffeeStore.Models.Exception;
using PRN232.Lab2.CoffeeStore.Models.Request.ZaloPay;
using PRN232.Lab2.CoffeeStore.Repositories.Entity;
using PRN232.Lab2.CoffeeStore.Repositories.UnitOfWork;
using PRN232.Lab2.CoffeeStore.Services.IService;

namespace PRN232.Lab2.CoffeeStore.Services.Service;

public class PaymentService : IPaymentService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly HttpClient _httpClient;
    private readonly ZaloPayConfig _zaloPayConfig;

    public PaymentService(IUnitOfWork unitOfWork, IOptions<ZaloPayConfig> zaloPayConfig)
    {
        _unitOfWork = unitOfWork;
        _httpClient = new HttpClient();
        _zaloPayConfig = zaloPayConfig.Value;
    }

    public async Task<string> GetPaymentUrlAsync(int orderId)
    {
        var order = await _unitOfWork.Orders.GetFirstOrDefaultAsync(
            o => o.OrderId == orderId,
            "OrderDetails"
        );
        if (order == null)
        {
            throw new AppException(ErrorCode.OrderNotFound);
        }else if(order.Status != nameof(OrderStatus.Pending))
        {
            throw new AppException(ErrorCode.OrderStatusError);
        }
        var totalAmount = order.OrderDetails.Sum(od => od.Quantity * od.UnitPrice);
        var payment = new Payment
        {
            OrderId = orderId,
            Amount = totalAmount,
            PaymentMethod = "ZaloPay",
            PaymentDate = DateTime.Now
        };
        await _unitOfWork.Payments.AddAsync(payment);
        await _unitOfWork.CompleteAsync();


        var formattedDate = payment.PaymentDate.ToString("yyMMddHHmmss");
        var appTransId = $"{formattedDate}_{orderId}_{payment.PaymentId}";
        var appTime = DateTimeOffset.Now.ToUnixTimeMilliseconds();
        var callBackUrl = _zaloPayConfig.CallBackUrl;
        var appUser = order.UserId.ToString();
        var amount = (long) totalAmount; // Amount in VND
        var embedData = "{}";
        var items = "[]";
        var description = $"Payment for order #{orderId}";

        var hmacInput = $"{_zaloPayConfig.AppId}|{appTransId}|{appUser}|{amount}|{appTime}|{embedData}|{items}";
        var mac = HmacSha256(hmacInput, _zaloPayConfig.Key1);

        var payload = new Dictionary<string, object>
        {
            { "app_id", _zaloPayConfig.AppId },
            { "app_trans_id", appTransId },
            { "app_user", appUser },
            { "app_time", appTime },
            { "amount", amount },
            { "description", description },
            { "embed_data", embedData },
            { "item", items },
            { "mac", mac },
            { "callback_url", callBackUrl }
        };
        var response = await _httpClient.PostAsJsonAsync(_zaloPayConfig.CreateOrderUrl, payload);
        var result = await response.Content.ReadFromJsonAsync<Dictionary<string, object>>();

        if (result == null || !result.TryGetValue("order_url", out var value))
        {
            Console.WriteLine($"ZaloPay Error: {await response.Content.ReadAsStringAsync()}");
            throw new AppException(ErrorCode.FailToGetPaymentUrl);
        }

        return value!.ToString()!;
    }

    public async Task<bool> VerifyPaymentAsync(ZaloPayCallbackRequest request)
    {
        try
        {
            var key2 = _zaloPayConfig.Key2;
            string calculatedMac = HmacSha256(request.Data, key2);

            if (calculatedMac != request.Mac)
            {
                return false;
            }

            var callbackData = JsonSerializer.Deserialize<ZaloPayCallbackData>(request.Data);
            if (callbackData == null)
            {
                return false;
            }

            long zpTransId = callbackData.ZpTransId;
            string paymentIdStr = callbackData.AppTransId;
            var parts = paymentIdStr.Split('_');
            if (parts.Length < 3)
            {
                return false;
            }

            int paymentId = int.Parse(parts[2]);
            var payment = await _unitOfWork.Payments.GetByIdAsync(paymentId);
            if (payment == null)
            {
                return false;
            }
            payment.Status = "COMPLETED";
            int orderId = int.Parse(parts[1]);
            var order = await _unitOfWork.Orders.GetByIdAsync(orderId);
            if (order == null)
            {
                return false;
            }
            order.Status = nameof(OrderStatus.Confirmed);
            await _unitOfWork.CompleteAsync();
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred while processing ZaloPay callback: {ex.Message}");
            return false;
        }
    }

    private static string HmacSha256(string data, string key)
    {
        using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(key));
        var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(data));
        // FIX: Chuyển byte array sang hex string
        return string.Concat(hash.Select(b => b.ToString("x2")));
    }
}