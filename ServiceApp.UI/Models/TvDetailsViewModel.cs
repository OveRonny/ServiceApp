using System.Text.Json.Serialization;

namespace ServiceApp.UI.Models;

public class TvDetailsViewModel
{
    public int TmdbId { get; set; }
    public string Title { get; set; } = "";
    public string Overview { get; set; } = "";
    public int SeasonNumber { get; set; }
    public DateTime? FirstAirDate { get; set; }
    public int NumberOfSeasons { get; set; }
    public int NumberOfEpisodes { get; set; }
    public double? Rating { get; set; }
    public string? PosterPath { get; set; }
    public string? PosterUrl =>
    !string.IsNullOrEmpty(PosterPath)
        ? $"https://image.tmdb.org/t/p/w500{PosterPath}"
        : null;

    [JsonPropertyName("genres")]
    public List<string> Genres { get; set; } = new();

    public List<TvSeasonViewModel> Seasons { get; set; } = new();
}

public class TvSeasonViewModel
{
    public int SeasonNumber { get; set; }

    public string Name { get; set; } = "";

    public string? Overview { get; set; }

    public int EpisodeCount { get; set; }

    public double? VoteAverage { get; set; }

    public string? PosterPath { get; set; }

    public string? PosterUrl =>
        string.IsNullOrEmpty(PosterPath)
            ? null
            : $"https://image.tmdb.org/t/p/w500{PosterPath}";

    public string? AirDate { get; set; }

    public DateTime? AirDateParsed =>
        string.IsNullOrEmpty(AirDate)
            ? null
            : DateTime.Parse(AirDate);
}

public class TvDetailsApiDto
{
    [JsonPropertyName("tmdbId")]
    public int TmdbId { get; set; }

    [JsonPropertyName("title")]
    public string Title { get; set; } = "";

    [JsonPropertyName("overview")]
    public string? Overview { get; set; }

    [JsonPropertyName("firstAirDate")]
    public string? FirstAirDate { get; set; }

    [JsonPropertyName("numberOfSeasons")]
    public int NumberOfSeasons { get; set; }

    [JsonPropertyName("numberOfEpisodes")]
    public int NumberOfEpisodes { get; set; }

    [JsonPropertyName("rating")]
    public double Rating { get; set; }

    [JsonPropertyName("posterPath")]
    public string? PosterPath { get; set; }

    [JsonPropertyName("genres")]
    public List<string> Genres { get; set; } = new();

    [JsonPropertyName("seasons")]
    public List<TvSeasonApiDto> Seasons { get; set; } = new();
}


public class TvSeasonApiDto
{
    [JsonPropertyName("season_number")]
    public int SeasonNumber { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; } = "";

    [JsonPropertyName("episode_count")]
    public int EpisodeCount { get; set; }

    [JsonPropertyName("air_date")]
    public string? AirDate { get; set; }

    [JsonPropertyName("poster_path")]
    public string? PosterPath { get; set; }

    public double VoteAverage { get; set; }
    public string? Overview { get; set; }
}










