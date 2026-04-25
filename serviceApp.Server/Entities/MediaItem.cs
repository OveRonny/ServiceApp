namespace serviceApp.Server.Entities;

public class MediaItem
{
    public int Id { get; set; }

    public string Title { get; set; } = string.Empty;
    public MediaType Type { get; set; }

    public string? Overview { get; set; }
    public DateTime? ReleaseDate { get; set; }

    public int? DurationMinutes { get; set; }
    public string? PosterPath { get; set; }

    public int? Seasons { get; set; }
    public int? Episodes { get; set; }
    public int? AverageEpisodeMinutes { get; set; }

    public int TmdbId { get; set; }
    public string? ImdbId { get; set; }

    public ICollection<MediaItemGenre> MediaItemGenres { get; set; } = new List<MediaItemGenre>();
    public ICollection<WatchHistory> WatchHistories { get; set; } = new List<WatchHistory>();
    public ICollection<WatchlistItem> WatchlistItems { get; set; } = new List<WatchlistItem>();
    public ICollection<Season> SeasonsNav { get; set; } = new List<Season>();
}
