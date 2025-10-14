using System.Security.Claims;
using PRN232.Lab2.CoffeeStore.Models.Request.Auth;
using PRN232.Lab2.CoffeeStore.Models.Request.User;
using PRN232.Lab2.CoffeeStore.Models.Response.Auth;
using PRN232.Lab2.CoffeeStore.Models.Response.User;
using PRN232.Lab2.CoffeeStore.Repositories.Entity;

namespace PRN232.Lab2.CoffeeStore.Services.IService;

public interface IAuthService
{
    Task<TokenResponse> Login(AuthRequest request);
    Task<UserResponse?> Register(UserRequest request);
    Task<(string AccessToken, string RefreshToken)> RefreshToken(string oldRefreshToken);
    Task RevokeRefreshToken(string refreshToken);
    Task<string> GenerateAccessToken(User user);
    Task<RefreshToken> GenerateRefreshToken(Guid userId);
}