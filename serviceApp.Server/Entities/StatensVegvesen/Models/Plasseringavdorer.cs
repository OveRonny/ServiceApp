using System.Text.Json.Serialization;

namespace serviceApp.Server.Entities.StatensVegvesen.Models;


public class Plasseringavdorer
{
    [JsonPropertyName("kodeBeskrivelse")]
    public string? KodeBeskrivelse { get; set; }

    [JsonPropertyName("kodeNavn")]
    public string? KodeNavn { get; set; }

    [JsonPropertyName("kodeTypeId")]
    public string? KodeTypeId { get; set; }

    [JsonPropertyName("kodeVerdi")]
    public string? KodeVerdi { get; set; }

    [JsonPropertyName("tidligereKodeVerdi")]
    public object[]? TidligereKodeVerdi { get; set; }
}
