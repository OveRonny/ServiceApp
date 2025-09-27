using System.Text.Json.Serialization;


namespace serviceApp.Server.Entities.StatensVegvesen.Models;

public class Eftypegodkjenning
{
    [JsonPropertyName("typegodkjenningNrTekst")]
    public string? TypeGodkjenningNrTekst { get; set; }

    [JsonPropertyName("typegodkjenningnummer")]
    public Typegodkjenningnummer? TypeGodkjenningNummer { get; set; }

    [JsonPropertyName("variant")]
    public string? Variant { get; set; }

    [JsonPropertyName("versjon")]
    public string? Versjon { get; set; }
}
