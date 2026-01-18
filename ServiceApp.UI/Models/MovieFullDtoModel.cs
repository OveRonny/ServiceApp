namespace ServiceApp.UI.Models;

public class MovieFullDtoModel
{
    public int TmdbId { get; set; }
    public int MediaItemId { get; set; }
    public string Title { get; set; } = "";
    public string? Overview { get; set; }
    public int? Runtime { get; set; }
    public DateTime? WatchedDate { get; set; }
    public List<string> Genres { get; set; } = new();
    public string? PosterPath { get; set; }
    // ===== Helpers =====
    public string? PosterUrl =>
        PosterPath is null ? null : $"https://image.tmdb.org/t/p/w500{PosterPath}";
}


