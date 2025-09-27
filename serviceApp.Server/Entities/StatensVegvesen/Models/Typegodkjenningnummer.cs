using System.Text.Json.Serialization;

namespace serviceApp.Server.Entities.StatensVegvesen.Models;

public class Typegodkjenningnummer
{
    [JsonPropertyName("direktiv")]
    public string? Direktiv { get; set; }

    [JsonPropertyName("land")]
    public string? Land { get; set; }

    [JsonPropertyName("serie")]
    public string? Serie { get; set; }

    [JsonPropertyName("utvidelse")]
    public string? Utvidelse { get; set; }
}