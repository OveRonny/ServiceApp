using System.Text.Json.Serialization;


namespace serviceApp.Server.Entities.StatensVegvesen.Models;

public class Miljodata
{
    [JsonPropertyName("euroKlasse")]
    public Euroklasse? EuroKlasse { get; set; }

    [JsonPropertyName("miljoOgDrivstoffGruppe")]
    public Miljoogdrivstoffgruppe[]? MiljoOgDrivstoffGruppe { get; set; }

    [JsonPropertyName("okoInnovasjon")]
    public bool OkoInnovasjon { get; set; }
}