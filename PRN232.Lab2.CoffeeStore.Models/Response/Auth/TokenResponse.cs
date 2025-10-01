namespace PRN232.Lab2.CoffeeStore.Models.Response.Auth;

public record TokenResponse(
    string AccessToken,
    string RefreshToken
);