using Microsoft.AspNetCore.Mvc;
using PRN232.Lab2.CoffeeStore.Models.Request.Auth;
using PRN232.Lab2.CoffeeStore.Models.Request.User;
using PRN232.Lab2.CoffeeStore.Models.Response.Common;
using PRN232.Lab2.CoffeeStore.Models.Response.User;
using PRN232.Lab2.CoffeeStore.Services.IService;

namespace PRN232.Lab2.CoffeeStore.API.Controller;

[ApiController]
[Route("/auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("login")]
    public async Task<ApiResponse<string>> Login([FromBody] AuthRequest request)
    {
        var token = await _authService.Login(request);

        return new ApiResponse<string>
        {
            StatusCode = 200,
            Data = token
        };
    }

    [HttpPost("register")]
    public async Task<ApiResponse<UserResponse>> Register([FromBody] UserRequest request)
    {
        var user = await _authService.Register(request);

        return new ApiResponse<UserResponse>
        {
            StatusCode = 201,
            Message = "Account Registered Successfully",
            Data = user
        };
    }
}