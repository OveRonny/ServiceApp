using System.Text.Json.Serialization;


namespace serviceApp.Server.Entities.StatensVegvesen.Models;

public class Forstegangsgodkjenning
{
    [JsonPropertyName("forstegangGodkjenning")]
    public string? ForstegangRegistrertDato { get; set; }

    [JsonPropertyName("fortollingOgMva")]
    public Fortollingogmva? FortollingOgMva { get; set; }

    [JsonPropertyName("godkjenningsId")]
    public string? GodkjenningsId { get; set; }

    [JsonPropertyName("godkjenningsundertype")]
    public Godkjenningsundertype? GodkjenningsUndertype { get; set; }

    [JsonPropertyName("gyldigFraDato")]
    public string? GyldigFraDato { get; set; }

    [JsonPropertyName("gyldigFraDatoTid")]
    public DateTime GyldigFraDatoTid { get; set; }

    [JsonPropertyName("unntak")]
    public object[]? Unntak { get; set; }
}
