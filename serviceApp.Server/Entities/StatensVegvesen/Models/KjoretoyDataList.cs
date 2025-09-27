using System.Text.Json.Serialization;

namespace serviceApp.Server.Entities.StatensVegvesen.Models;

public class KjoretoyDataList
{
    [JsonPropertyName("kjoretoydataListe")]
    public Kjoretoydata[]? KjoretoydataListe { get; set; }

}
