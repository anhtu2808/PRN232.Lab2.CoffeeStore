using System.Net;

namespace PRN232.Lab2.CoffeeStore.Models.Exception;

public class ErrorCode
{
    public HttpStatusCode StatusCode { get; set; }
    public string Message { get; set; } = string.Empty;

    public ErrorCode(HttpStatusCode statusCode, string message)
    {
        StatusCode = statusCode;
        Message = message;
    }


    //404 Not Found
    public static readonly ErrorCode ProductNotFound = new(HttpStatusCode.NotFound, "Product not found.");
    public static readonly ErrorCode CategoryNotFound = new(HttpStatusCode.NotFound, "Category not found.");
    public static readonly ErrorCode OrderNotFound = new(HttpStatusCode.NotFound, "Order not found.");
    public static readonly ErrorCode UserNotFound = new(HttpStatusCode.NotFound, "User not found.");


    //400 Bad Request
    public static readonly ErrorCode ProductAlreadyInactive =
        new(HttpStatusCode.BadRequest, "Product is already inactive.");

    public static readonly ErrorCode UsernameAlreadyExists =
        new(HttpStatusCode.BadRequest, "Username already exists.");

    public static readonly ErrorCode EmailAlreadyExists =
        new(HttpStatusCode.BadRequest, "Email already exists.");

    //401 Unauthorized
    public static readonly ErrorCode Unauthorized = new(HttpStatusCode.Unauthorized, "Unauthorized access.");

    public static readonly ErrorCode InvalidUsernameOrPassword =
        new(HttpStatusCode.Unauthorized, "Invalid username or password.");

    //403 Forbidden
}