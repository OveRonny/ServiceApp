using System.Text.Json.Serialization;

namespace serviceApp.Server.Entities.StatensVegvesen.Models;

public class Kravomrade
{
    [JsonPropertyName("kodeBeskrivelse")]
    public string? KodeBeskrivelse { get; set; }

    [JsonPropertyName("kodeVerdi")]
    public string? KodeVerdi { get; set; }

    [JsonPropertyName("tidligereKodeVerdi")]
    public object[]? TidligereKodeVerdi { get; set; }
}
