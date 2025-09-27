using System.Text.Json.Serialization;

namespace serviceApp.Server.Entities.StatensVegvesen.Models;

public class Kjoretoymerknad
{
    [JsonPropertyName("merknad")]
    public string? Merknad { get; set; }

    [JsonPropertyName("merknadtypeKode")]
    public string? MerknadTypeKode { get; set; }
}
