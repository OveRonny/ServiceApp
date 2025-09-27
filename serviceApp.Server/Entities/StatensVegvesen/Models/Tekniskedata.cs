using serviceApp.Server.Entities.StatensVegvesen.Models;
using System.Text.Json.Serialization;


namespace serviceApp.Server.Entities.StatensVegvesen.Models;

public class Tekniskedata
{
    [JsonPropertyName("akslinger")]
    public Akslinger? Akslinger { get; set; }

    [JsonPropertyName("bremser")]
    public Bremser? Bremser { get; set; }

    [JsonPropertyName("dekkOgFelg")]
    public Dekkogfelg? DekkOgFelg { get; set; }

    [JsonPropertyName("dimensjoner")]
    public Dimensjoner? Dimensjoner { get; set; }

    [JsonPropertyName("generelt")]
    public Generelt? Generelt { get; set; }

    [JsonPropertyName("karosseriOgLasteplan")]
    public Karosserioglasteplan? KarosseriOgLasteplan { get; set; }

    [JsonPropertyName("miljodata")]
    public Miljodata? MiljoData { get; set; }

    [JsonPropertyName("motorOgDrivverk")]
    public Motorogdrivverk? MotorOgDrivverk { get; set; }

    [JsonPropertyName("ovrigeTekniskeData")]
    public object[]? OvrigeTekniskeData { get; set; }

    [JsonPropertyName("persontall")]
    public Persontall? Persontall { get; set; }

    [JsonPropertyName("tilhengerkopling")]
    public Tilhengerkopling? TilhengerKopling { get; set; }

    [JsonPropertyName("vekter")]
    public Vekter? Vekter { get; set; }
}
