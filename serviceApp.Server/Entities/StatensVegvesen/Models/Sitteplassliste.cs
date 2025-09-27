using System.Text.Json.Serialization;


namespace serviceApp.Server.Entities.StatensVegvesen.Models;

public class Sitteplassliste
{
    [JsonPropertyName("sitteplass")]
    public Sitteplass[]? SittePlass { get; set; }
}