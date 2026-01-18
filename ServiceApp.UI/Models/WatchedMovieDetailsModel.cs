namespace ServiceApp.UI.Models;

public class WatchedMovieDetailsModel
{
    public int MediaItemId { get; set; }     // Database ID
    public int TmdbId { get; set; }           // TMDb ID
    public string Title { get; set; } = string.Empty;
    public string? Overview { get; set; }
    public int? Runtime { get; set; }
    public List<string> Genres { get; set; } = new();
    public string? PosterPath { get; set; }
    public string? MediaType { get; set; }
    public string? ReleaseDate { get; set; }

    // Extra info from your database
    public List<DateTime>? WatchDates { get; set; }
    public double? LastProgress { get; set; }

    public List<WatchHistoryModel> WatchHistory { get; set; } = new();
}


