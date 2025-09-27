using System.Text.Json.Serialization;

namespace serviceApp.Server.Entities.StatensVegvesen.Models;

public class Tekniskkode1
{
    [JsonPropertyName("kodeBeskrivelse")]
    public string? KodeBeskrivelse { get; set; }

    [JsonPropertyName("kodeNavn")]
    public string? KodeNavn { get; set; }

    [JsonPropertyName("kodeVerdi")]
    public string? KodeVerdi { get; set; }

    [JsonPropertyName("tidligereKodeVerdi")]
    public object[]? TidligereKodeVerdi { get; set; }
}