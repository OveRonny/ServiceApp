using System.Text.Json.Serialization;

namespace serviceApp.Server.Entities.StatensVegvesen.Models;

public class Fabrikant
{
    [JsonPropertyName("fabrikantNavn")]
    public string? FabrikantNavn { get; set; }
}
