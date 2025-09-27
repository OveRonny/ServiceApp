using serviceApp.Server.Entities.StatensVegvesen.Models;
using System.Text.Json.Serialization;


namespace serviceApp.Server.Entities.StatensVegvesen.Models;

public class Dekkogfelg
{
    [JsonPropertyName("akselDekkOgFelgKombinasjon")]
    public Akseldekkogfelgkombinasjon[]? AkselDekkOgFelgKombinasjon { get; set; }
}
