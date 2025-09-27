using System.Text.Json.Serialization;


namespace serviceApp.Server.Entities.StatensVegvesen.Models;


public class Miljoogdrivstoffgruppe
{

    [JsonPropertyName("drivstoffKodeMiljodata")]
    public Drivstoffkodemiljodata? DrivstoffKodeMiljodata { get; set; }

    [JsonPropertyName("forbrukOgUtslipp")]
    public Forbrukogutslipp[]? ForbrukOgUtslipp { get; set; }

    [JsonPropertyName("lyd")]
    public Lyd? Lyd { get; set; }
}
