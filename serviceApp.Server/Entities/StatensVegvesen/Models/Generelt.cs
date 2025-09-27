using System.Text.Json.Serialization;


namespace serviceApp.Server.Entities.StatensVegvesen.Models;

public class Generelt
{
    [JsonPropertyName("fabrikant")]
    public Fabrikant[]? Fabrikant { get; set; }

    [JsonPropertyName("handelsbetegnelse")]
    public string[]? HandelsBetegnelse { get; set; }

    [JsonPropertyName("merke")]
    public Merke[]? Merke { get; set; }

    [JsonPropertyName("tekniskKode")]
    public Tekniskkode1? TekniskKode { get; set; }

    [JsonPropertyName("typebetegnelse")]
    public string? TypeBetegnelse { get; set; }
}