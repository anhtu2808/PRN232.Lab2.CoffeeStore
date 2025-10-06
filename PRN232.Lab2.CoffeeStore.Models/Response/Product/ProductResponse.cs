using System.Text.Json.Serialization;

namespace PRN232.Lab2.CoffeeStore.Models.Response.Product;

public record ProductResponse(
    int ProductId,
    string Name,
    string? Description,
    decimal Price,
    bool IsActive,
    [property: JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    int? CategoryId,
    [property: JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    string? CategoryName
);