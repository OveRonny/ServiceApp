using System.Text.Json.Serialization;


namespace serviceApp.Server.Entities.StatensVegvesen.Models;

public class Akslinger
{
    [JsonPropertyName("akselGruppe")]
    public Akselgruppe[]? AkselGruppe { get; set; }

    [JsonPropertyName("antallAksler")]
    public int AntallAksler { get; set; }
}
