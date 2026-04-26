using System.Text.Json.Serialization;

namespace serviceApp.Server.Features.Tmdb;



#region TMDb Paged Response

public class TmdbPagedResponse<T>
{
    [JsonPropertyName("page")]
    public int Page { get; set; }

    [JsonPropertyName("results")]
    public List<T> Results { get; set; } = new();

    [JsonPropertyName("total_pages")]
    public int TotalPages { get; set; }

    [JsonPropertyName("total_results")]
    public int TotalResults { get; set; }
}

#endregion

#region TMDb Search Result (Movie + TV)

public class TmdbSearchResultDto
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    // Movie title
    [JsonPropertyName("title")]
    public string? Title { get; set; }

    [JsonPropertyName("overview")]
    public string? Overview { get; set; }

    // TV name
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("media_type")]
    public string MediaType { get; set; } = "";

    [JsonPropertyName("release_date")]
    public string? ReleaseDate { get; set; }

    [JsonPropertyName("first_air_date")]
    public string? FirstAirDate { get; set; }

    [JsonPropertyName("poster_path")]
    public string? PosterPath { get; set; }

    // ===== Helpers =====

    [JsonIgnore]
    public string DisplayTitle => Title ?? Name ?? "Unknown";

    [JsonIgnore]
    public int? Year
    {
        get
        {
            var date = ReleaseDate ?? FirstAirDate;
            return DateTime.TryParse(date, out var d) ? d.Year : null;
        }
    }

    [JsonIgnore]
    public string? PosterUrl =>
        PosterPath is null ? null : $"https://image.tmdb.org/t/p/w500{PosterPath}";
}

#endregion

#region TMDb Movie Details

public class TmdbMovieDetailsDto
{
    [JsonPropertyName("id")]
    public int Id { get; set; }
    public int TmdbId { get; set; }

    [JsonPropertyName("title")]
    public string Title { get; set; } = "";

    [JsonPropertyName("overview")]
    public string? Overview { get; set; }

    [JsonPropertyName("runtime")]
    public int? Runtime { get; set; }

    [JsonPropertyName("release_date")]
    public string? ReleaseDate { get; set; }

    [JsonPropertyName("genres")]
    public List<TmdbGenreDto> Genres { get; set; } = new();

    [JsonPropertyName("poster_path")]
    public string? PosterPath { get; set; }

    [JsonPropertyName("imdb_id")]
    public string? ImdbId { get; set; }

    public bool InWatchlist { get; set; } = false;
    public DateTime? LastWatchedDate { get; set; }
    public double? LastProgress { get; set; }

    public List<WatchHistoryListDto> WatchHistory { get; set; } = new();


    // ===== Helpers =====

    [JsonIgnore]
    public string? PosterUrl =>
        PosterPath is null ? null : $"https://image.tmdb.org/t/p/w500{PosterPath}";
}

public class WatchHistoryListDto
{
    public DateTime WatchDate { get; set; }
    public bool Liked { get; set; }
    public int? Rating { get; set; }
}

#endregion

#region TMDb TV Details

public class TmdbTvDetailsDto
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    public int TmdbId { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; } = "";

    [JsonPropertyName("original_name")]
    public string OriginalName { get; set; } = "";

    [JsonPropertyName("overview")]
    public string? Overview { get; set; }

    [JsonPropertyName("first_air_date")]
    public string? FirstAirDate { get; set; }

    [JsonPropertyName("last_air_date")]
    public string? LastAirDate { get; set; }

    [JsonPropertyName("number_of_seasons")]
    public int NumberOfSeasons { get; set; }

    [JsonPropertyName("number_of_episodes")]
    public int NumberOfEpisodes { get; set; }

    [JsonPropertyName("genres")]
    public List<TmdbGenreDto> Genres { get; set; } = new();

    [JsonPropertyName("created_by")]
    public List<TmdbCreatorDto> CreatedBy { get; set; } = new();

    [JsonPropertyName("poster_path")]
    public string? PosterPath { get; set; }

    [JsonPropertyName("backdrop_path")]
    public string? BackdropPath { get; set; }

    [JsonPropertyName("seasons")]
    public List<TmdbSeasonDto> Seasons { get; set; } = new();

    [JsonPropertyName("vote_average")]
    public double VoteAverage { get; set; }

    [JsonPropertyName("external_ids")]
    public TmdbExternalIds? ExternalIds { get; set; }

    // ===== Helpers =====

    [JsonIgnore]
    public string? PosterUrl =>
        string.IsNullOrEmpty(PosterPath) ? null : $"https://image.tmdb.org/t/p/w500{PosterPath}";

    [JsonIgnore]
    public string? BackdropUrl =>
        string.IsNullOrEmpty(BackdropPath) ? null : $"https://image.tmdb.org/t/p/w780{BackdropPath}";

    [JsonIgnore]
    public DateTime? FirstAirDateParsed =>
        string.IsNullOrEmpty(FirstAirDate) ? null : DateTime.Parse(FirstAirDate);

    [JsonIgnore]
    public DateTime? LastAirDateParsed =>
        string.IsNullOrEmpty(LastAirDate) ? null : DateTime.Parse(LastAirDate);
}



public class TmdbCreatorDto
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; } = "";

    [JsonPropertyName("original_name")]
    public string OriginalName { get; set; } = "";

    [JsonPropertyName("profile_path")]
    public string? ProfilePath { get; set; }

    [JsonIgnore]
    public string? ProfileUrl =>
        string.IsNullOrEmpty(ProfilePath) ? null : $"https://image.tmdb.org/t/p/w185{ProfilePath}";
}

public class TmdbSeasonDto
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("season_number")]
    public int SeasonNumber { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; } = "";

    [JsonPropertyName("overview")]
    public string? Overview { get; set; }

    [JsonPropertyName("poster_path")]
    public string? PosterPath { get; set; }

    [JsonPropertyName("episode_count")]
    public int EpisodeCount { get; set; }

    [JsonPropertyName("air_date")]
    public string? AirDate { get; set; }

    [JsonPropertyName("vote_average")]
    public double? VoteAverage { get; set; }

    [JsonIgnore]
    public string? PosterUrl =>
        string.IsNullOrEmpty(PosterPath) ? null : $"https://image.tmdb.org/t/p/w500{PosterPath}";

    [JsonIgnore]
    public DateTime? AirDateParsed =>
        string.IsNullOrEmpty(AirDate) ? null : DateTime.Parse(AirDate);
}

public class TmdbExternalIds
{
    [JsonPropertyName("imdb_id")]
    public string? ImdbId { get; set; }
}

#endregion

#region TMDb Shared DTOs

public class TmdbGenreDto
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; } = "";
}



#endregion
#region TV Series List DTOs

public class TvSeriesListDto
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

public class TvWatchStatusDto
{
    public int MediaItemId { get; set; }
    public bool IsInWatchlist { get; set; }
    public List<int> WatchedSeasonNumbers { get; set; } = new();
    public List<int> WatchedEpisodeIds { get; set; } = new();
    /// <summary>SeasonNumber → antall sette episoder</summary>
    public Dictionary<int, int> WatchedEpisodeCountBySeason { get; set; } = new();
    public int? StreamingService { get; set; }
    public string? Comment { get; set; }
}

#endregion
#region TV Episode DTOs

public class TmdbSeasonApiDto
{
    [JsonPropertyName("season_number")]
    public int SeasonNumber { get; set; }

    [JsonPropertyName("episodes")]
    public List<TmdbEpisodeApiDto> Episodes { get; set; } = new();
}

public class TmdbEpisodeApiDto
{
    [JsonPropertyName("id")]
    public int TmdbEpisodeId { get; set; }

    [JsonPropertyName("episode_number")]
    public int EpisodeNumber { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; } = "";

    [JsonPropertyName("overview")]
    public string? Overview { get; set; }

    [JsonPropertyName("air_date")]
    public string? AirDate { get; set; }

    [JsonPropertyName("vote_average")]
    public double? VoteAverage { get; set; }

    [JsonPropertyName("still_path")]
    public string? StillPath { get; set; }
}

public class SeasonEpisodesDto
{
    public int SeasonNumber { get; set; }
    public List<EpisodeWithStatusDto> Episodes { get; set; } = new();
}

public class EpisodeWithStatusDto
{
    public int Id { get; set; }
    public int EpisodeNumber { get; set; }
    public string Name { get; set; } = "";
    public string? Overview { get; set; }
    public double? VoteAverage { get; set; }
    public string? StillPath { get; set; }
    public bool IsWatched { get; set; }
    public DateTime? WatchedDate { get; set; }

    public string? StillUrl =>
        string.IsNullOrEmpty(StillPath) ? null : $"https://image.tmdb.org/t/p/w300{StillPath}";
}

#endregion
#region Movie DTOs
public class MovieFullDto
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

public class PagedResult<T>
{
    public IReadOnlyList<T> Items { get; init; } = [];
    public int Total { get; init; }
    public int Page { get; init; }
    public int PageSize { get; init; }
}

#endregion



