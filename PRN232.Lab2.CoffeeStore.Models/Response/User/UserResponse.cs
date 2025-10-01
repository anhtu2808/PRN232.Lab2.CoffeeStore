namespace PRN232.Lab2.CoffeeStore.Models.Response.User;

public class UserResponse
{
    public Guid UserId { get; set; }

    public string Username { get; set; } = null!;

    public string Email { get; set; } = null!;
    
    public DateTime CreatedDate { get; set; }
}