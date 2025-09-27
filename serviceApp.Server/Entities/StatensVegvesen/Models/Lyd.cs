using System.Text.Json.Serialization;


namespace serviceApp.Server.Entities.StatensVegvesen.Models;

public class Lyd
{
    [JsonPropertyName("kjorestoy")]
    public int KjoreStoy { get; set; }

    [JsonPropertyName("standstoy")]
    public int StandStoy { get; set; }

    [JsonPropertyName("stoyMalingOppgittAv")]
    public Stoymalingoppgittav? StoyMalingOppgittAv { get; set; }

    [JsonPropertyName("vedAntallOmdreininger")]
    public int VedAntallOmdreininger { get; set; }
}
