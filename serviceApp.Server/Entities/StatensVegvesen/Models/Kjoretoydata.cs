using System.Text.Json.Serialization;


namespace serviceApp.Server.Entities.StatensVegvesen.Models;

public class Kjoretoydata
{
    [JsonPropertyName("kjoretoyId")]
    public KjoretoyId? KjoretoyId { get; set; }

    [JsonPropertyName("forstegangsregistrering")]
    public Forstegangsregistrering? Forstegangsregistrering { get; set; }

    [JsonPropertyName("kjennemerke")]
    public Kjennemerke[]? Kjennemerke { get; set; }

    [JsonPropertyName("registrering")]
    public Registrering? Registrering { get; set; }

    [JsonPropertyName("godkjenning")]
    public Godkjenning? Godkjenning { get; set; }

    [JsonPropertyName("periodiskKjoretoyKontroll")]
    public PeriodiskKjoretoyKontroll? PeriodiskKjoretoyKontroll { get; set; }
}