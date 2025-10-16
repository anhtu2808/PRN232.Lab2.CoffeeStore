using PRN232.Lab2.CoffeeStore.Models.Request.Common;

namespace PRN232.Lab2.CoffeeStore.Models.Request.User;
public class UserFilterRequest : RequestParameters
{
    public string? Username { get; set; }
    public string? Email { get; set; }
    public DateTime? CreatedDateFrom { get; set; }
    public DateTime? CreatedDateTo { get; set; }
}