using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using AutoMapper;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using PRN232.Lab2.CoffeeStore.Models.Exception;
using PRN232.Lab2.CoffeeStore.Models.Request.Auth;
using PRN232.Lab2.CoffeeStore.Models.Request.User;
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

    public async Task<string> Login(AuthRequest request)
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var user = await _unitOfWork.Users.GetByUsernameAndPasswordAsync(request.Username, request.Password);
        if (user == null)
        {
            throw new AppException(ErrorCode.InvalidUsernameOrPassword);
        }

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.UserId.ToString()),
            new Claim(JwtRegisteredClaimNames.Name, user.Username),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddMinutes(double.Parse(_configuration["Jwt:AccessTokenExpirationMinutes"]!)),
            Issuer = _configuration["Jwt:Issuer"],
            Audience = _configuration["Jwt:Audience"],
            SigningCredentials = credentials
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);

        return tokenHandler.WriteToken(token);
    }

    public string CreateRefreshToken()
    {
        var randomNumber = new byte[64];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }

    public ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
    {
        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateAudience = false,
            ValidateIssuer = false,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!)),
            ValidateLifetime = false
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out var securityToken);

        if (securityToken is not JwtSecurityToken jwtSecurityToken ||
            !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256,
                StringComparison.InvariantCultureIgnoreCase))
        {
            throw new SecurityTokenException("Invalid token");
        }

        return principal;
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
            CreatedDate = DateTime.UtcNow
        };

        await _unitOfWork.Users.AddAsync(newUser);
        await _unitOfWork.CompleteAsync();

        return _mapper.Map<UserResponse>(newUser);
    }
}