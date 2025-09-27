using System.Text.Json.Serialization;

namespace serviceApp.Server.Entities.StatensVegvesen.Models;

public class Bremser
{
    [JsonPropertyName("abs")]
    public bool Abs { get; set; }

    [JsonPropertyName("tilhengerBremseforbindelse")]
    public object[]? TilhengerBremseforbindelse { get; set; }
}

