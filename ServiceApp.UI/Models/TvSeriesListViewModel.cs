using System.Text.Json.Serialization;

namespace ServiceApp.UI.Models;

public class TvSeriesListViewModel
{
    public int MediaItemId { get; set; }
    public int TmdbId { get; set; }
    public string Title { get; set; } = "";
    public string? PosterPath { get; set; }
    public int? TotalSeasons { get; set; }
    public int WatchedSeasons { get; set; }
    public DateTime? LastWatchedDate { get; set; }
    public DateTime? AddedToWatchlistAt { get; set; }

    public string? PosterUrl =>
        string.IsNullOrEmpty(PosterPath) ? null : $"https://image.tmdb.org/t/p/w500{PosterPath}";
}

public class TvWatchStatusViewModel
{
    [JsonPropertyName("mediaItemId")]
    public int MediaItemId { get; set; }

    [JsonPropertyName("isInWatchlist")]
    public bool IsInWatchlist { get; set; }

    [JsonPropertyName("watchedSeasonNumbers")]
    public List<int> WatchedSeasonNumbers { get; set; } = new();

    [JsonPropertyName("watchedEpisodeIds")]
    public List<int> WatchedEpisodeIds { get; set; } = new();

    [JsonPropertyName("watchedEpisodeCountBySeason")]
    public Dictionary<int, int> WatchedEpisodeCountBySeason { get; set; } = new();

    [JsonPropertyName("streamingService")]
    public int? StreamingService { get; set; }

    [JsonPropertyName("comment")]
    public string? Comment { get; set; }
}

public class SeasonEpisodeViewModel
{
    public int SeasonNumber { get; set; }
    public List<EpisodeViewModel> Episodes { get; set; } = new();
}

public class EpisodeViewModel
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("episodeNumber")]
    public int EpisodeNumber { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; } = "";

    [JsonPropertyName("overview")]
    public string? Overview { get; set; }

    [JsonPropertyName("voteAverage")]
    public double? VoteAverage { get; set; }

    [JsonPropertyName("isWatched")]
    public bool IsWatched { get; set; }

    [JsonPropertyName("watchedDate")]
    public DateTime? WatchedDate { get; set; }
}
