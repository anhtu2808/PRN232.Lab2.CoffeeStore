using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PRN232.Lab2.CoffeeStore.Models.Request.Common;
using PRN232.Lab2.CoffeeStore.Models.Request.Payment;
using PRN232.Lab2.CoffeeStore.Models.Request.ZaloPay;
using PRN232.Lab2.CoffeeStore.Models.Response.Common;
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


    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetPayments(
        [FromQuery] PaymentFilter filter
    )
    {
        var pagePayment = await _paymentService.GetPaymentsAsync(filter);

        var response = new ApiResponse<object>
        {
            StatusCode = 200,
            Message = "Payments retrieved successfully.",
            Data = pagePayment
        };

        return Ok(response);
    }


    [HttpGet("/zalopay-link")]
    [Authorize]
    public async Task<IActionResult> GetPaymentUrl([FromQuery] int orderId)
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