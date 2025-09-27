using System.Text.Json.Serialization;

namespace serviceApp.Server.Entities.StatensVegvesen.Models;

public class Fortollingogmva
{
    [JsonPropertyName("fortollingsreferanse")]
    public string? FortollingsReferanse { get; set; }
}
