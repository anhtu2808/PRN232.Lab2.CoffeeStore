using System.Text.Json.Serialization;

namespace PRN232.Lab2.CoffeeStore.Models.Request.ZaloPay;

public class ZaloPayCallbackData
{
    [JsonPropertyName("app_id")] public int AppId { get; set; }

    [JsonPropertyName("app_trans_id")] public string AppTransId { get; set; }

    [JsonPropertyName("app_time")] public long AppTime { get; set; }

    [JsonPropertyName("app_user")] public string AppUser { get; set; }

    [JsonPropertyName("amount")] public long Amount { get; set; }

    [JsonPropertyName("embed_data")] public string EmbedData { get; set; }

    [JsonPropertyName("item")] public string Item { get; set; }

    [JsonPropertyName("zp_trans_id")] public long ZpTransId { get; set; }

    [JsonPropertyName("server_time")] public long ServerTime { get; set; }

    [JsonPropertyName("channel")] public int Channel { get; set; }
}