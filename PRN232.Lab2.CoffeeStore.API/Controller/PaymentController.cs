using Microsoft.AspNetCore.Mvc;
using PRN232.Lab2.CoffeeStore.Models.Request.ZaloPay;
using PRN232.Lab2.CoffeeStore.Services.IService;

namespace PRN232.Lab2.CoffeeStore.API.Controller;

[ApiController]
[Route("payments")]
public class PaymentController : ControllerBase
{
    private readonly IPaymentService _paymentService;
    private readonly ILogger<PaymentController> _logger;

    public PaymentController(IPaymentService paymentService, ILogger<PaymentController> logger)
    {
        _paymentService = paymentService;
        _logger = logger;
    }


    [HttpPost("{orderId}/create")]
    public async Task<IActionResult> CreatePaymentUrl([FromRoute] int orderId)
    {
        var url = await _paymentService.GetPaymentUrlAsync(orderId);
        return Ok(new
        {
            OrderId = orderId,
            PaymentUrl = url
        });
    }


    [HttpPost("callback")] 
    public async Task<IActionResult> ZaloPayReturn([FromBody] ZaloPayCallbackRequest request)
    {
        // Gọi service để xác thực chữ ký và dữ liệu callback
        var verificationResult = await _paymentService.VerifyPaymentAsync(request);
        if (verificationResult)
        {
            // Xác thực thành công, chuyển hướng về trang thành công
            return Redirect("https://your-frontend-url.com/payment-success");
        }
        else
        {
            // Xác thực thất bại, chuyển hướng về trang thất bại
            return Redirect("https://your-frontend-url.com/payment-failure");
        }
    }


    
}