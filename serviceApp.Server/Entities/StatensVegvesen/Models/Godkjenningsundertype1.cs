using System.Text.Json.Serialization;

namespace serviceApp.Server.Entities.StatensVegvesen.Models;

public class Godkjenningsundertype1
{
    [JsonPropertyName("kodeNavn")]
    public string? KodeNavn { get; set; }

    [JsonPropertyName("kodeVerdi")]
    public string? KodeVerdi { get; set; }

    [JsonPropertyName("tidligereKodeVerdi")]
    public object[]? TidligereKodeVerdi { get; set; }
}

