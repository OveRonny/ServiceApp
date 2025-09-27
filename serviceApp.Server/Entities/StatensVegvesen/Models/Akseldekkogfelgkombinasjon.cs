using System.Text.Json.Serialization;

namespace serviceApp.Server.Entities.StatensVegvesen.Models;

public class Akseldekkogfelgkombinasjon
{
    [JsonPropertyName("akselDekkOgFelg")]
    public Akseldekkogfelg[]? AkselDekkOgFelg { get; set; }
}