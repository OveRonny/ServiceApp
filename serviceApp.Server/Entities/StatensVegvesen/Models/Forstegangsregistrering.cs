using System.Text.Json.Serialization;

namespace serviceApp.Server.Entities.StatensVegvesen.Models;

public class Forstegangsregistrering
{
    [JsonPropertyName("registrertForstegangNorgeDato")]
    public string? RegistrertForstegangNorgeDato { get; set; }
}
