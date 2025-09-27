using System.Text.Json.Serialization;

namespace serviceApp.Server.Entities.StatensVegvesen.Models;

public class Registreringsbegrensninger
{
    [JsonPropertyName("registreringsbegrensning")]
    public object[]? RegistreringsBegrensning { get; set; }
}