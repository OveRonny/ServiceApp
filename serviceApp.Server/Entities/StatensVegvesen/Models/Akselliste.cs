using System.Text.Json.Serialization;


namespace serviceApp.Server.Entities.StatensVegvesen.Models;

public class Akselliste
{
    [JsonPropertyName("aksel")]
    public Aksel[]? Aksel { get; set; }
}
