using System.Text.Json.Serialization;


namespace serviceApp.Server.Entities.StatensVegvesen.Models;

public class Karosserioglasteplan
{
    [JsonPropertyName("antallDorer")]
    public int[]? AntallDorer { get; set; }

    [JsonPropertyName("dorUtforming")]
    public object[]? DorUtforming { get; set; }

    [JsonPropertyName("karosseritype")]
    public Karosseritype? KarosseriType { get; set; }

    [JsonPropertyName("kjennemerketypeBak")]
    public Kjennemerketypebak? KjennemerkeTypeBak { get; set; }

    [JsonPropertyName("kjennemerkestorrelseBak")]
    public Kjennemerkestorrelsebak? KjennemerkeStorrelseBak { get; set; }

    [JsonPropertyName("kjennemerketypeForan")]
    public Kjennemerketypeforan? KjennemerkeTypeForan { get; set; }

    [JsonPropertyName("kjennemerkestorrelseForan")]
    public Kjennemerkestorrelseforan? KjennemerkeStorrelseForan { get; set; }

    [JsonPropertyName("kjoringSide")]
    public string? KjoringSide { get; set; }

    [JsonPropertyName("oppbygningUnderstellsnummer")]
    public string? OppbygningUnderstellsNummer { get; set; }

    [JsonPropertyName("plasseringAvDorer")]
    public Plasseringavdorer? PlasseringAvDorer { get; set; }

    [JsonPropertyName("plasseringFabrikasjonsplate")]
    public Plasseringfabrikasjonsplate[]? PlasseringFabrikasjonsPlate { get; set; }

    [JsonPropertyName("plasseringUnderstellsnummer")]
    public Plasseringunderstellsnummer[]? PlasseringUnderstellsNummer { get; set; }

    [JsonPropertyName("rFarge")]
    public Rfarge[]? RFarge { get; set; }
}
