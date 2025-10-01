using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using PRN232.Lab2.CoffeeStore.Models.Exception;

namespace PRN232.Lab2.CoffeeStore.API.Configuration
{
    public static class AuthenticationServiceConfiguration
    {
        public static IServiceCollection AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            var jwtSection = configuration.GetSection("Jwt");
            var key = jwtSection.GetValue<string>("Key");

            if (string.IsNullOrEmpty(key))
                throw new InvalidOperationException("JWT Key is missing in appsettings.json");

            services
                .AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(options =>
                {
                    options.RequireHttpsMetadata = false;
                    options.SaveToken = true;
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = jwtSection.GetValue<string>("Issuer"),
                        ValidAudience = jwtSection.GetValue<string>("Audience"),
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key))
                    };

                    options.Events = new JwtBearerEvents
                    {
                        OnChallenge = async context =>
                        {
                            context.HandleResponse(); 

                            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                            context.Response.ContentType = "application/json";

                            var result = new
                            {
                                statusCode = 401,
                                message = "Unauthorized - Invalid or missing token"
                            };

                            await context.Response.WriteAsJsonAsync(result);
                        },
                        OnForbidden = async context =>
                        {
                            context.Response.StatusCode = StatusCodes.Status403Forbidden;
                            context.Response.ContentType = "application/json";

                            var result = new
                            {
                                statusCode = 403,
                                message = "Forbidden - You don't have permission"
                            };

                            await context.Response.WriteAsJsonAsync(result);
                        }
                    };

                });

            return services;
        }
    }
}
