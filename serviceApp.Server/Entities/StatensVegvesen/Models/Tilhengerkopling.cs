using System.Text.Json.Serialization;

namespace serviceApp.Server.Entities.StatensVegvesen.Models;

public class Tilhengerkopling
{
    [JsonPropertyName("kopling")]
    public object[]? Kopling { get; set; }
}
