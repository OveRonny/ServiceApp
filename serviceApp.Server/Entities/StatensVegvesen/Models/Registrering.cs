using System.Text.Json.Serialization;


namespace serviceApp.Server.Entities.StatensVegvesen.Models;

public class Registrering
{
    [JsonPropertyName("fomTidspunkt")]
    public DateTime FomTidspunkt { get; set; }
    [JsonPropertyName("kjoringensArt")]
    public Kjoringensart? KjoringensArt { get; set; }
    [JsonPropertyName("registreringsstatus")]
    public Registreringsstatus? RegistreringsStatus { get; set; }
    [JsonPropertyName("registrertForstegangPaEierskap")]
    public DateTime RegistrertForstegangPaEierskap { get; set; }
}