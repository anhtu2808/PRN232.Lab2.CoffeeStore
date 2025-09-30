namespace PRN232.Lab2.CoffeeStore.Models.Response;

public record TokenResponse(
    string AccessToken,
    string RefreshToken
);