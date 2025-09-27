using System.Text.Json.Serialization;


namespace serviceApp.Server.Entities.StatensVegvesen.Models;

public class Tekniskgodkjenning
{
    [JsonPropertyName("godkjenningsId")]
    public string? GodkjenningsId { get; set; }

    [JsonPropertyName("godkjenningsundertype")]
    public Godkjenningsundertype1? GodkjenningsUndertype { get; set; }

    [JsonPropertyName("gyldigFraDato")]
    public string? GyldigFraDato { get; set; }

    [JsonPropertyName("gyldigFraDatoTid")]
    public DateTime GyldigFraDatoTid { get; set; }

    [JsonPropertyName("kjoretoyklassifisering")]
    public Kjoretoyklassifisering? Kjoretoyklassifisering { get; set; }

    [JsonPropertyName("krav")]
    public Krav[]? Krav { get; set; }

    [JsonPropertyName("tekniskeData")]
    public Tekniskedata? TekniskeData { get; set; }

    [JsonPropertyName("unntak")]
    public object[]? Unntak { get; set; }
}

