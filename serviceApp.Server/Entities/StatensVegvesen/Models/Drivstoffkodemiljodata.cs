using System.Text.Json.Serialization;

namespace serviceApp.Server.Entities.StatensVegvesen.Models;

public class Drivstoffkodemiljodata
{
    [JsonPropertyName("kodeBeskrivelse")]
    public string? KodeBeskrivelse { get; set; }
    public string? KodeNavn { get; set; }

    [JsonPropertyName("kodeTypeId")]
    public string? KodeTypeId { get; set; }

    [JsonPropertyName("kodeVerdi")]
    public string? KodeVerdi { get; set; }

    [JsonPropertyName("tidligereKodeVerdi")]
    public object[]? TidligereKodeVerdi { get; set; }
}
