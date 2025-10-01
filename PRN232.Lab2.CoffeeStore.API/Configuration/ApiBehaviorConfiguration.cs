using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using PRN232.Lab2.CoffeeStore.Models.Response.Common;

namespace PRN232.Lab2.CoffeeStore.API.Configuration;

public static class ApiBehaviorConfiguration
{
    public static IServiceCollection AddCustomApiBehavior(this IServiceCollection services)
    {
        services.AddControllers()
            .ConfigureApiBehaviorOptions(options =>
            {
                options.InvalidModelStateResponseFactory = context =>
                {
                    var errors = context.ModelState
                        .Where(x => x.Value.Errors.Count > 0)
                        .ToDictionary(
                            kvp => char.ToLowerInvariant(kvp.Key[0]) + kvp.Key.Substring(1),
                            kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).ToArray()
                        );

                    var response = new ApiResponse<object>
                    {
                        StatusCode = 400,
                        Message = "Validation failed",
                        Data = errors
                    };

                    return new BadRequestObjectResult(response);
                };
            })
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
            });

        return services;
    }
}