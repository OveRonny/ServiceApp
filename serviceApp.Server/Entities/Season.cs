namespace serviceApp.Server.Entities;

public class Season
{
    public int Id { get; set; }

    public int SeasonNumber { get; set; }
    public string Name { get; set; } = "";
    public string? Overview { get; set; }
    public string? PosterPath { get; set; }
    public DateTime? AirDate { get; set; }
    public double? VoteAverage { get; set; }
    public int? EpisodeCount { get; set; }
    // Relasjon til serien
    public int MediaItemId { get; set; }
    public MediaItem MediaItem { get; set; } = null!;

    // Episoder
    public ICollection<Episode> Episodes { get; set; } = new List<Episode>();
}
