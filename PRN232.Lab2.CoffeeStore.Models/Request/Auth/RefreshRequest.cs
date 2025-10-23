using System.ComponentModel.DataAnnotations;

namespace PRN232.Lab2.CoffeeStore.Models.Request.Auth;

public class RefreshRequest
{
    [Required(ErrorMessage = "Refresh token is required")]
    public String RefreshToken { get; set; } = string.Empty;
}