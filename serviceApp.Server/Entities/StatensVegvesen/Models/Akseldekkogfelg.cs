using System.Text.Json.Serialization;

namespace serviceApp.Server.Entities.StatensVegvesen.Models;

public class Akseldekkogfelg
{
    [JsonPropertyName("akselId")]
    public int AkselId { get; set; }

    [JsonPropertyName("belastningskodeDekk")]
    public string? BelastningskodeDekk { get; set; }

    [JsonPropertyName("dekkdimensjon")]
    public string? Dekkdimensjon { get; set; }

    [JsonPropertyName("felgdimensjon")]
    public string? FelgDimensjon { get; set; }

    [JsonPropertyName("hastighetskodeDekk")]
    public string? HastighetsKodeDekk { get; set; }

    [JsonPropertyName("innpress")]
    public string? Innpress { get; set; }

    [JsonPropertyName("tvilling")]
    public bool Tvilling { get; set; }
}
