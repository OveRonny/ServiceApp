using System.Text.Json.Serialization;


namespace serviceApp.Server.Entities.StatensVegvesen.Models;

public class Godkjenning
{
    [JsonPropertyName("forstegangsGodkjenning")]
    public Forstegangsgodkjenning? ForstegangsGodkjenning { get; set; }
    [JsonPropertyName("kjoretoymerknad")]
    public Kjoretoymerknad[]? KjoretoyMerknad { get; set; }
    [JsonPropertyName("registreringsbegrensninger")]
    public Registreringsbegrensninger? RegistreringsBegrensninger { get; set; }
    [JsonPropertyName("tekniskGodkjenning")]
    public Tekniskgodkjenning? TekniskGodkjenning { get; set; }
    [JsonPropertyName("tilleggsgodkjenninger")]
    public object[]? TilleggsGodkjenninger { get; set; }
}