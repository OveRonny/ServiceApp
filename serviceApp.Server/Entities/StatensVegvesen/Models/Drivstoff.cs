using System.Text.Json.Serialization;


namespace serviceApp.Server.Entities.StatensVegvesen.Models;

public class Drivstoff
{
    [JsonPropertyName("drivstoffKode")]
    public Drivstoffkode? DrivstoffKode { get; set; }

    [JsonPropertyName("maksNettoEffekt")]
    public float MaksNettoEffekt { get; set; }
}