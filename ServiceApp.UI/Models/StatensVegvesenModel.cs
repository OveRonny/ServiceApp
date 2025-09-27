namespace ServiceApp.UI.Models;

public class StatensVegvesenModel
{
    public string Merke { get; set; } = string.Empty;
    public string Modell { get; set; } = string.Empty;
    public int Ar { get; set; }
    public string Registreringsnummer { get; set; } = string.Empty;
    public string Farge { get; set; } = string.Empty;
    public string Eier { get; set; } = string.Empty;

    public string KontrollFrist { get; set; } = string.Empty;
    public string SistGodkjent { get; set; } = string.Empty;

    public string UnderstellsNummer { get; set; } = string.Empty;
    public DateTime RegistreringEier { get; set; }
    public string Dekkdimensjon { get; set; } = string.Empty;
    public string FelgDimensjon { get; set; } = string.Empty;
    public string Innpress { get; set; } = string.Empty;
    public string? MotorKode { get; set; }
    public string? KodeNavn { get; set; }
    public int? Vekt { get; set; }
    public string? FargeNavn { get; set; }
    public string? FargeKode { get; set; }
    public string? FargeBeskrivelse { get; set; }
    public string? GirKasse { get; set; }

    public int? Lengde { get; set; }
    public int? Hoyde { get; set; }
    public int? Bredde { get; set; }
    public float? Hestekrefter { get; set; }
    public int? Seter { get; set; }
    public int? Dorer { get; set; }
    public int? TilhengervektMedBremser { get; set; }
    public int? TilhengervektUtenBremser { get; set; }
    public string? ForrsteGangRegistrert { get; set; }
}
