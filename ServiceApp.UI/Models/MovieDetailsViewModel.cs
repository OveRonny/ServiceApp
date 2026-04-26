namespace ServiceApp.UI.Models;

public class MovieDetailsViewModel
{
    public int TmdbId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Overview { get; set; }
    public int? Runtime { get; set; }
    public List<string> Genres { get; set; } = new();
    public string? PosterPath { get; set; }
    public string? PosterUrl =>
    !string.IsNullOrEmpty(PosterPath)
        ? $"https://image.tmdb.org/t/p/w500{PosterPath}"
        : null;
    public string? ReleaseDate { get; set; }
    public string MediaType { get; set; } = "movie";

    public List<WatchHistoryModel> WatchHistory { get; set; } = new();

    public bool IsImported { get; set; }
    public bool InWatchlist { get; set; }
    public DateTime? LastWatchedDate { get; set; }

    public double? LastProgress { get; set; }

    public bool IsWatched =>
        LastProgress >= 100 || LastWatchedDate != null;

    public bool MarkAsWatched { get; set; } = false;

    public bool IsInMyWatchlist => InWatchlist;

    public int? StreamingService { get; set; }
    public string? Comment { get; set; }
}


public class WatchHistoryModel
{
    public DateTime? WatchDate { get; set; }
    public double Progress { get; set; }
    public bool? Liked { get; set; }
    public int? Rating { get; set; }
    public string? Comment { get; set; }
}