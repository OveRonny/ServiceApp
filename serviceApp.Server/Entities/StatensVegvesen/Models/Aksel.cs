using System.Text.Json.Serialization;

namespace serviceApp.Server.Entities.StatensVegvesen.Models;


public class Aksel
{
    [JsonPropertyName("antallHjul")]
    public int AntallHjul { get; set; }

    [JsonPropertyName("avstandTilNesteAksling")]
    public int AvstandTilNesteAksling { get; set; }

    [JsonPropertyName("drivAksel")]
    public bool DrivAksel { get; set; }

    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("plasseringAksel")]
    public string? PlasseringAksel { get; set; }

    [JsonPropertyName("sporvidde")]
    public int Sporvidde { get; set; }

    [JsonPropertyName("styreAksel")]
    public bool StyreAksel { get; set; }

    [JsonPropertyName("tekniskTillattAkselLast")]
    public int TekniskTillattAkselLast { get; set; }
}
