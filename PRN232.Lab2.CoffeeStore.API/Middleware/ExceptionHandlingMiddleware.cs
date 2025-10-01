using System.Net;
using System.Text.Json;
using PRN232.Lab2.CoffeeStore.Models.Exception;

namespace PRN232.Lab2.CoffeeStore.API.middleware;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (AppException ex)
        {
            _logger.LogError(ex, ex.Message);

            context.Response.StatusCode = (int)ex.StatusCode;
            context.Response.ContentType = "application/json";

            var result = JsonSerializer.Serialize(new
            {
                statusCode = context.Response.StatusCode,
                message = ex.Message
            });

            await context.Response.WriteAsync(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception occurred");

            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            context.Response.ContentType = "application/json";

            var result = JsonSerializer.Serialize(new
            {
                statusCode = context.Response.StatusCode,
                message = ex.Message
            });

            await context.Response.WriteAsync(result);
        }
    }
}