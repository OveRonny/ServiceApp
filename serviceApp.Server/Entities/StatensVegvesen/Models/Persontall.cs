using System.Text.Json.Serialization;


namespace serviceApp.Server.Entities.StatensVegvesen.Models;

public class Persontall
{
    [JsonPropertyName("sitteplassListe")]
    public Sitteplassliste? SittePlassListe { get; set; }

    [JsonPropertyName("sitteplasserForan")]
    public int SittePlasserForan { get; set; }

    [JsonPropertyName("sitteplasserTotalt")]
    public int SittePlasserTotalt { get; set; }
}