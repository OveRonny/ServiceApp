using System.Text.Json.Serialization;

namespace serviceApp.Server.Entities.StatensVegvesen.Models;


public class Nasjonalgodkjenning
{
    [JsonPropertyName("nasjonaltGodkjenningsAr")]
    public string? NasjonaltGodkjenningsAr { get; set; }

    [JsonPropertyName("nasjonaltGodkjenningsHovednummer")]
    public string? NasjonaltGodkjenningsHovednummer { get; set; }

    [JsonPropertyName("nasjonaltGodkjenningsUndernummer")]
    public string? NasjonaltGodkjenningsUndernummer { get; set; }
}
