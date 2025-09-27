using System.Text.Json.Serialization;


namespace serviceApp.Server.Entities.StatensVegvesen.Models;

public class Kjoretoyklassifisering
{
    [JsonPropertyName("beskrivelse")]
    public string? Beskrivelse { get; set; }

    [JsonPropertyName("efTypegodkjenning")]
    public Eftypegodkjenning? EfTypeGodkjenning { get; set; }

    [JsonPropertyName("kjoretoyAvgiftsKode")]
    public Kjoretoyavgiftskode? KjoretoyAvgiftsKode { get; set; }

    [JsonPropertyName("nasjonalGodkjenning")]
    public Nasjonalgodkjenning? NasjonalGodkjenning { get; set; }

    [JsonPropertyName("spesielleKjennetegn")]
    public string? SpesielleKjennetegn { get; set; }

    [JsonPropertyName("tekniskKode")]
    public Tekniskkode? TekniskKode { get; set; }

    [JsonPropertyName("tekniskUnderkode")]
    public Tekniskunderkode? TekniskUnderkode { get; set; }

    [JsonPropertyName("iSamsvarMedTypegodkjenning")]
    public bool ISamsvarMedTypegodkjenning { get; set; }
}
