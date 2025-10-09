using PRN232.Lab2.CoffeeStore.Models.Request.Common;

namespace PRN232.Lab2.CoffeeStore.Models.Request.Order;

public class OrderFilter : RequestParameters
{
    public Guid? UserId { get; set; }
    public int? Status { get; set; }
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
}