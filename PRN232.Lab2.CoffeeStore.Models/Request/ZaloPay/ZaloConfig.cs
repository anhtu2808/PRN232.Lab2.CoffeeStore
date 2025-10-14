namespace PRN232.Lab2.CoffeeStore.Models.Request.ZaloPay;

public class ZaloPayConfig
{
    public int AppId { get; set; }
    public string Key1 { get; set; } = string.Empty;
    public string Key2 { get; set; } = string.Empty;
    public string CreateOrderUrl { get; set; } = string.Empty;
    public string GetStatusUrl { get; set; } = string.Empty;

    public string CallBackUrl { get; set; } = string.Empty;
}