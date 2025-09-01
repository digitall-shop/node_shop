using System.Text.Json.Serialization;


namespace Application.Models.Safir;

public class GetRateResponse
{
    [JsonPropertyName("usd")] public long Usd { get; set; }
    
    [JsonPropertyName("eur")] public long Eur { get; set; }
    
    [JsonPropertyName("created_at")] public DateTime CreateAt { get; set; }
}