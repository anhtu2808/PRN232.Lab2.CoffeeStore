using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using PRN232.Lab2.CoffeeStore.Models.Request.Auth;
using PRN232.Lab2.CoffeeStore.Models.Request.User;
using PRN232.Lab2.CoffeeStore.Models.Response.Auth;
using PRN232.Lab2.CoffeeStore.Models.Response.Common;
using PRN232.Lab2.CoffeeStore.Models.Response.User;
using PRN232.Lab2.CoffeeStore.Services.IService;

namespace PRN232.Lab2.CoffeeStore.API.Controller;

[ApiController]
[Route("auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] AuthRequest request)
    {
        var tokenResponse = await _authService.Login(request);

        var response = new ApiResponse<TokenResponse>
        {
            StatusCode = 200,
            Message = "Login successful",
            Data = tokenResponse
        };

        return Ok(response);
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh([FromBody] RefreshRequest request)
    {
        var result = await _authService.RefreshToken(request.RefreshToken);

        var response = new ApiResponse<object>
        {
            StatusCode = 200,
            Message = "Token refreshed successfully",
            Data = new
            {
                accessToken = result.AccessToken,
                refreshToken = result.RefreshToken
            }
        };

        return Ok(response);
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] UserRequest request)
    {
        var user = await _authService.Register(request);

        var response = new ApiResponse<UserResponse>
        {
            StatusCode = 201,
            Message = "Account registered successfully",
            Data = user
        };


        return CreatedAtAction(
            nameof(Register),
            new { id = user.UserId },
            response
        );
    }
}