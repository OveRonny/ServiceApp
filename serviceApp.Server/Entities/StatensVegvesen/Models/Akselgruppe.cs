using serviceApp.Server.Entities.StatensVegvesen.Models;
using System.Text.Json.Serialization;


namespace serviceApp.Server.Entities.StatensVegvesen.Models;

public class Akselgruppe
{
    [JsonPropertyName("akselListe")]
    public Akselliste? AkselListe { get; set; }

    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("plasseringAkselGruppe")]
    public string? PlasseringAkselGruppe { get; set; }

    [JsonPropertyName("tekniskTillattAkselGruppeLast")]
    public int TekniskTillattAkselGruppeLast { get; set; }
}
