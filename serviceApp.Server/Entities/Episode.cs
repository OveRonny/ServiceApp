namespace serviceApp.Server.Entities;

public class Episode
{
    public int Id { get; set; }

    public int EpisodeNumber { get; set; }
    public string Name { get; set; } = "";
    public string? Overview { get; set; }
    public DateTime? AirDate { get; set; }
    public double? VoteAverage { get; set; }

    // Relasjon til sesong
    public int SeasonId { get; set; }
    public Season Season { get; set; } = null!;
}
