using System.Security.Claims;
using PRN232.Lab2.CoffeeStore.Models.Exception;

namespace PRN232.Lab2.CoffeeStore.API.Utils;

public class JwtUtils
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public JwtUtils(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }


    public string GetUserIdFromToken()
    {
        var user = _httpContextAccessor.HttpContext?.User;

        if (user == null || !user.Identity?.IsAuthenticated == true)
            throw new AppException(ErrorCode.InvalidToken);

        // Thử lấy theo chuẩn ClaimTypes.NameIdentifier (được map với "sub")
        var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        // Nếu không có, thử lấy trực tiếp claim "sub"
        if (string.IsNullOrEmpty(userId))
            userId = user.FindFirst("sub")?.Value;

        if (string.IsNullOrEmpty(userId))
            throw new Exception("User ID (sub) not found in token");

        return userId;
    }
    
    public List<string> GetCurrentUserRoles()
    {
        var user = _httpContextAccessor.HttpContext?.User;
        if (user == null)
            return new List<string>();

        return user.Claims
            .Where(c => c.Type == ClaimTypes.Role || c.Type == "role")
            .Select(c => c.Value)
            .ToList();
    }
    
    public string? GetCurrentUserRole()
    {
        return GetCurrentUserRoles().FirstOrDefault();
    }
}