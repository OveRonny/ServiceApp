using System.Text.Json.Serialization;

namespace serviceApp.Server.Entities.StatensVegvesen.Models;

public class Sitteplass
{
    [JsonPropertyName("beltestrammer")]
    public bool BelteStrammer { get; set; }
}