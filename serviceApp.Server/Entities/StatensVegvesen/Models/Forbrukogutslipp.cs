using System.Text.Json.Serialization;


namespace serviceApp.Server.Entities.StatensVegvesen.Models;

public class Forbrukogutslipp
{
    [JsonPropertyName("co2BlandetKjoring")]
    public float Co2BlandetKjoring { get; set; }

    [JsonPropertyName("forbrukBlandetKjoring")]
    public float ForbrukBlandetKjoring { get; set; }

    [JsonPropertyName("malemetode")]
    public Malemetode? MaleMetode { get; set; }

    [JsonPropertyName("partikkelfilterFabrikkmontert")]
    public bool PartikkelFilterFabrikkmontert { get; set; }

    [JsonPropertyName("utslippCOmgPrKm")]
    public float UtslippCOmgPrKm { get; set; }

    [JsonPropertyName("utslippNOxMgPrKm")]
    public float UtslippNOxMgPrKm { get; set; }

    [JsonPropertyName("utslippPartikkelAntallPrKm")]
    public float UtslippPartikkelAntallPrKm { get; set; }

    [JsonPropertyName("utslippPartiklerMgPrKm")]
    public float UtslippPartiklerMgPrKm { get; set; }

    [JsonPropertyName("utslippTHCogNOxMgPrKm")]
    public float UtslippTHCogNOxMgPrKm { get; set; }

}
