using System.Text.Json.Serialization;

namespace serviceApp.Server.Entities.StatensVegvesen.Models;

public class Merke
{
    [JsonPropertyName("merke")]
    public string? MerkeNavn { get; set; }

    [JsonPropertyName("merkeKode")]
    public string? MerkeKode { get; set; }
}
