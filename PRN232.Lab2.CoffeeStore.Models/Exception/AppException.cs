namespace PRN232.Lab2.CoffeeStore.Models.Exception;

public class AppException : System.Exception
{
    public System.Net.HttpStatusCode StatusCode { get; set; } 
        = System.Net.HttpStatusCode.InternalServerError;

    public AppException() { }

    public AppException(string message) : base(message) { }

    public AppException(string message, System.Exception inner) 
        : base(message, inner) { }

    public AppException(ErrorCode errorCode) : base(errorCode.Message)
    {
        StatusCode = errorCode.StatusCode;
    }
}