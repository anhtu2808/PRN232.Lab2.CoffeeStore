namespace PRN232.Lab2.CoffeeStore.Models.Response.Auth;

public class TokenResponse(string accessToken, string refreshToken)
{
    public string AccessToken { get; set; } = accessToken;
    public string RefreshToken { get; set; } = refreshToken;

    public TokenResponse() : this(string.Empty, string.Empty)
    {
    }
}