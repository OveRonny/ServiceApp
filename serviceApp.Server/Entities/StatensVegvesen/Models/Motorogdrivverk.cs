using System.Text.Json.Serialization;


namespace serviceApp.Server.Entities.StatensVegvesen.Models;

public class Motorogdrivverk
{
    [JsonPropertyName("girkassetype")]
    public Girkassetype? GirkasseType { get; set; }

    [JsonPropertyName("girutvekslingPrGir")]
    public object[]? GirUtvekslingPrGir { get; set; }

    [JsonPropertyName("hybridkategori")]
    public Hybridkategori? HybridKategori { get; set; }

    [JsonPropertyName("maksimumHastighet")]
    public int[]? MaksimumHastighet { get; set; }

    [JsonPropertyName("maksimumHastighetMalt")]
    public object[]? MaksimumHastighetMalt { get; set; }

    [JsonPropertyName("motor")]
    public Motor[]? Motor { get; set; }

    [JsonPropertyName("obd")]
    public bool Obd { get; set; }
}