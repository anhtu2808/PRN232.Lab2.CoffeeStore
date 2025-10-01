using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;

namespace PRN232.Lab2.CoffeeStore.API.Configuration;

public static class SwaggerServiceConfiguration
{
    public static IServiceCollection AddSwaggerWithJwt(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(c =>
        {
            // JWT bearer config
            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Description = "Enter 'Bearer' [space] and then your valid token.\nExample: \"Bearer eyJhbGci...\"",
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT"
            });

            c.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    Array.Empty<string>()
                }
            });
        });

        return services;
    }
}
