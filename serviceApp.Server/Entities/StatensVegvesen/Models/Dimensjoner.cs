using System.Text.Json.Serialization;

namespace serviceApp.Server.Entities.StatensVegvesen.Models;


public class Dimensjoner
{
    [JsonPropertyName("bredde")]
    public int Bredde { get; set; }

    [JsonPropertyName("hoyde")]
    public int Hoyde { get; set; }

    [JsonPropertyName("lengde")]
    public int Lengde { get; set; }

    [JsonPropertyName("lengdeInnvendigLasteplan")]
    public int LengdeInnvendigLasteplan { get; set; }
}