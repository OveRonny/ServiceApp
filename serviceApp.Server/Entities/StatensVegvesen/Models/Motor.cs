using System.Text.Json.Serialization;


namespace serviceApp.Server.Entities.StatensVegvesen.Models;

public class Motor
{
    [JsonPropertyName("antallSylindre")]
    public int AntallSylindre { get; set; }

    [JsonPropertyName("arbeidsprinsipp")]
    public Arbeidsprinsipp? ArbeidsPrinsipp { get; set; }

    [JsonPropertyName("drivstoff")]
    public Drivstoff[]? DrivStoff { get; set; }

    [JsonPropertyName("motorKode")]
    public string? MotorKode { get; set; }

    [JsonPropertyName("partikkelfilterMotor")]
    public bool PartikkelFilterMotor { get; set; }

    [JsonPropertyName("slagvolum")]
    public int Slagvolum { get; set; }

    [JsonPropertyName("sylinderarrangement")]
    public Sylinderarrangement? SylinderArrangement { get; set; }
}
