using System.Text.Json.Serialization;

namespace PRN232.Lab2.CoffeeStore.Models.Response.Common;

public class ApiResponse<T>
{
    public int StatusCode { get; set; }
    public string Message { get; set; } = null!;
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public T? Data { get; set; }
}