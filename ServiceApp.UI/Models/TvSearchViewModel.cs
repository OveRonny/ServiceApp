namespace ServiceApp.UI.Models;

public class TvSearchViewModel
{
    public class TvResult
    {
        public int TmdbId { get; set; }

        // TMDb bruker 'name' for TV-serier
        public string Name { get; set; } = "";

        // Parse string til DateTime?
        public DateTime? FirstAirDate { get; set; }
        public string Overview { get; set; } = string.Empty;

        // vote_average fra TMDb
        public double? Rating { get; set; }

        public string PosterUrl { get; set; } = "";
    }
}
