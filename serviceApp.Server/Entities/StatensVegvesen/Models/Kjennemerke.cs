using System.Text.Json.Serialization;


namespace serviceApp.Server.Entities.StatensVegvesen.Models;

public class Kjennemerke
{
    [JsonPropertyName("fomTidspunkt")]
    public DateTime FomTidspunkt { get; set; }

    [JsonPropertyName("kjennemerke")]
    public string? KjennemerkeNavn { get; set; }

    [JsonPropertyName("kjennemerkekategori")]
    public string? KjenneMerkeKategori { get; set; }

    [JsonPropertyName("kjennemerketype")]
    public Kjennemerketype? KjenneMerkeType { get; set; }
}
