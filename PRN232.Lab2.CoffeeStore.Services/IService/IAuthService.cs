using System.Security.Claims;
using PRN232.Lab2.CoffeeStore.Models.Request.Auth;
using PRN232.Lab2.CoffeeStore.Models.Request.User;
using PRN232.Lab2.CoffeeStore.Models.Response.User;
using PRN232.Lab2.CoffeeStore.Repositories.Entity;

namespace PRN232.Lab2.CoffeeStore.Services.IService;

public interface IAuthService
{
    Task<string> Login(AuthRequest request);
    string CreateRefreshToken();
    ClaimsPrincipal GetPrincipalFromExpiredToken(string token);
    
    Task<UserResponse?> Register(UserRequest request);
}