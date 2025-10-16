using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using AutoMapper;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using PRN232.Lab2.CoffeeStore.Models.Enums;
using PRN232.Lab2.CoffeeStore.Models.Exception;
using PRN232.Lab2.CoffeeStore.Models.Request.Auth;
using PRN232.Lab2.CoffeeStore.Models.Request.User;
using PRN232.Lab2.CoffeeStore.Models.Response.Auth;
using PRN232.Lab2.CoffeeStore.Models.Response.User;
using PRN232.Lab2.CoffeeStore.Repositories.Entity;
using PRN232.Lab2.CoffeeStore.Repositories.UnitOfWork;
using PRN232.Lab2.CoffeeStore.Services.IService;
using JwtRegisteredClaimNames = System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames;

namespace PRN232.Lab2.CoffeeStore.Services.Service;

public class AuthService : IAuthService
{
    private readonly IConfiguration _configuration;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public AuthService(IConfiguration configuration, IUnitOfWork unitOfWork, IMapper mapper)
    {
        _configuration = configuration;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<TokenResponse> Login(AuthRequest request)
    {
        var user = await _unitOfWork.Users.GetFirstOrDefaultAsync(u =>
            u.Username == request.Username);
        if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
        {
            throw new AppException(ErrorCode.InvalidUsernameOrPassword);
        }
        var tokenResponse = new TokenResponse
        {
            AccessToken = await GenerateAccessToken(user),
            RefreshToken = (await GenerateRefreshToken(user.UserId)).Token
        };

        return tokenResponse;
    }

    public async Task<UserResponse?> Register(UserRequest request)
    {
        var existingUser = await _unitOfWork.Users.GetFirstOrDefaultAsync(u => u.Username == request.Username);
        if (existingUser != null)
        {
            throw new AppException(ErrorCode.UsernameAlreadyExists);
        }

        existingUser = await _unitOfWork.Users.GetFirstOrDefaultAsync(u => u.Email == request.Email);
        if (existingUser != null)
        {
            throw new AppException(ErrorCode.EmailAlreadyExists);
        }

        var newUser = new User
        {
            UserId = Guid.NewGuid(),
            Username = request.Username,
            Email = request.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            Role = nameof(Role.Customer),
            CreatedDate = DateTime.UtcNow
        };

        await _unitOfWork.Users.AddAsync(newUser);
        await _unitOfWork.CompleteAsync();

        return _mapper.Map<UserResponse>(newUser);
    }

    public async Task<(string AccessToken, string RefreshToken)> RefreshToken(string oldRefreshToken)
    {
        var existing = await _unitOfWork.RefreshTokens.GetFirstOrDefaultAsync(x => x.Token == oldRefreshToken);

        if (existing == null || existing.ExpiresAt <= DateTime.UtcNow)
            throw new AppException(ErrorCode.Unauthorized);

        var user = await _unitOfWork.Users.GetByIdAsync(existing.UserId);
        if (user == null)
            throw new AppException(ErrorCode.Unauthorized);

        await _unitOfWork.RefreshTokens.DeleteAsync(existing);

        var newRefreshToken = await GenerateRefreshToken(user.UserId);
        var newAccessToken = GenerateAccessToken(user);

        await _unitOfWork.CompleteAsync();

        return (newAccessToken.Result, newRefreshToken.Token);
    }


    public async Task RevokeRefreshToken(string refreshToken)
    {
        var token = await _unitOfWork.RefreshTokens.GetFirstOrDefaultAsync(x => x.Token == refreshToken);
        if (token != null)
        {
            await _unitOfWork.RefreshTokens.DeleteAsync(token);
            await _unitOfWork.CompleteAsync();
        }
    }


    public Task<string> GenerateAccessToken(User user)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.UserId.ToString()),
            new Claim(JwtRegisteredClaimNames.Name, user.Username),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(ClaimTypes.Role, user.Role)
        };

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddMinutes(double.Parse(_configuration["Jwt:AccessTokenExpirationMinutes"]!)),
            Issuer = _configuration["Jwt:Issuer"],
            Audience = _configuration["Jwt:Audience"],
            SigningCredentials = creds
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return Task.FromResult(tokenHandler.WriteToken(token));
    }

    public async Task<RefreshToken> GenerateRefreshToken(Guid userId)
    {
        var randomBytes = new byte[64];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomBytes);

        var refreshToken = new RefreshToken
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Token = Convert.ToBase64String(randomBytes),
            ExpiresAt = DateTime.UtcNow.AddDays(double.Parse(_configuration["Jwt:RefreshTokenExpirationDays"]!)),
            CreatedAt = DateTime.UtcNow
        };

        await _unitOfWork.RefreshTokens.AddAsync(refreshToken);
        await _unitOfWork.CompleteAsync();

        return refreshToken;
    }
}