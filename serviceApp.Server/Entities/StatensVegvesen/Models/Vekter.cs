using System.Text.Json.Serialization;

namespace serviceApp.Server.Entities.StatensVegvesen.Models;

public class Vekter
{
    [JsonPropertyName("egenvekt")]
    public int EgenVekt { get; set; }

    [JsonPropertyName("egenvektMinimum")]
    public int EgenvektMinimum { get; set; }

    [JsonPropertyName("nyttelast")]
    public int Nyttelast { get; set; }

    [JsonPropertyName("tillattTilhengervektMedBrems")]
    public int TillattTilhengervektMedBrems { get; set; }

    [JsonPropertyName("tillattTilhengervektUtenBrems")]
    public int TillattTilhengervektUtenBrems { get; set; }

    [JsonPropertyName("tillattTotalvekt")]
    public int TillattTotalvekt { get; set; }

    [JsonPropertyName("tillattVertikalKoplingslast")]
    public int TillattVertikalKoplingslast { get; set; }

    [JsonPropertyName("tillattVogntogvekt")]
    public int TillattVogntogvekt { get; set; }

    [JsonPropertyName("vogntogvektAvhBremsesystem")]
    public object[]? VogntogVektAvhBremsesystem { get; set; }
}