using System.Text.Json.Serialization;

namespace serviceApp.Server.Entities.StatensVegvesen.Models;

public class PeriodiskKjoretoyKontroll
{
    [JsonPropertyName("kontrollfrist")]
    public string? KontrollFrist { get; set; }

    [JsonPropertyName("sistGodkjent")]
    public string? SistGodkjent { get; set; }
}
