namespace PRN232.Lab2.CoffeeStore.Services.Models.Response;

public record TokenResponse(
    string AccessToken,
    string RefreshToken
);