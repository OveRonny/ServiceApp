using System.Text.Json.Serialization;


namespace serviceApp.Server.Entities.StatensVegvesen.Models;

public class Krav
{
    [JsonPropertyName("kravomrade")]
    public Kravomrade? Kravomrade { get; set; }

    [JsonPropertyName("kravoppfyllelse")]
    public Kravoppfyllelse? KravOppfyllelse { get; set; }
}