namespace ServiceApp.UI.Models;

public class MovieSearchViewModel
{
    // Search input
    public string Query { get; set; } = string.Empty;

    // Results from TMDb API
    public List<MovieResult> Results { get; set; } = new List<MovieResult>();

    // Loading state
    public bool IsLoading { get; set; }

    // Error message
    public string? ErrorMessage { get; set; }

    // Nested class for each movie in the results
    public class MovieResult
    {
        public int TmdbId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Overview { get; set; } = string.Empty;
        public string? PosterPath { get; set; }
        public string? PosterUrl => PosterPath != null ? $"https://image.tmdb.org/t/p/w500{PosterPath}" : null;
        public int? Year { get; set; }
        public string MediaType { get; set; } = "movie"; // "movie" or "tv"
    }
}
