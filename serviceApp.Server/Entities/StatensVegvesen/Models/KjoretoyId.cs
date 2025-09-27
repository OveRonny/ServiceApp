using System.Text.Json.Serialization;

namespace serviceApp.Server.Entities.StatensVegvesen.Models;

public class KjoretoyId
{
    [JsonPropertyName("kjennemerke")]
    public string? KjenneMerke { get; set; }
    [JsonPropertyName("understellsnummer")]
    public string? UnderstellsNummer { get; set; }
}
