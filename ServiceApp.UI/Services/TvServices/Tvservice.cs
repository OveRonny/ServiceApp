
using ServiceApp.UI.Mappers;
using ServiceApp.UI.Models;
using System.Net.Http.Json;

namespace ServiceApp.UI.Services.TvServices;

public class TvService(IHttpClientFactory http) : ITvservice
{
    private readonly IHttpClientFactory http = http;

    private HttpClient ApiAuthed() => http.CreateClient("ApiAuthed");

    // =========================
    // Search TV
    // =========================
    public async Task<List<TvSearchViewModel.TvResult>> SearchTvAsync(string query)
    {
        var http = ApiAuthed();
        var url = $"api/tmdb/search-tv?query={Uri.EscapeDataString(query)}";

        var response = await http.GetFromJsonAsync<List<TvSearchViewModel.TvResult>>(url);

        return response?.Select(r => new TvSearchViewModel.TvResult
        {
            TmdbId = r.TmdbId,
            Name = r.Name,
            PosterUrl = r.PosterUrl,
            FirstAirDate = r.FirstAirDate,
            Overview = r.Overview,
            Rating = r.Rating
        }).ToList() ?? new List<TvSearchViewModel.TvResult>();
    }

    // =========================
    // Get TV Details
    // =========================
    public async Task<TvDetailsViewModel?> GetTvDetailsAsync(int tmdbId)
    {
        var http = ApiAuthed();
        var response = await http.GetAsync($"api/tmdb/tv/{tmdbId}");

        if (!response.IsSuccessStatusCode)
            return null;

        var dto = await response.Content.ReadFromJsonAsync<TvDetailsApiDto>();
        return dto == null ? null : TestMapper.Map(dto);
    }


    // =========================
    // Import TV (+ adds to watchlist)
    // =========================
    public async Task<(bool Success, bool AlreadyInWatchlist)> ImportTvAsync(int tmdbId)
    {
        var http = ApiAuthed();
        var result = await http.PostAsJsonAsync("api/tmdb/import/tv", new { Tmdb = tmdbId });

        if (!result.IsSuccessStatusCode)
            return (false, false);

        var response = await result.Content.ReadFromJsonAsync<ImportTvResponse>();
        return response is null ? (false, false) : (true, response.AlreadyInWatchlist);
    }

    // =========================
    // Watched TV Series
    // =========================
    public async Task<PagedResult<TvSeriesListViewModel>> GetWatchedTvSeriesAsync(int page = 1, int pageSize = 24, string? search = null)
    {
        var http = ApiAuthed();
        var url = BuildPagedUrl("api/tmdb/tv/watched", page, pageSize, search);
        var response = await http.GetAsync(url);
        if (!response.IsSuccessStatusCode)
            return EmptyPaged<TvSeriesListViewModel>(page, pageSize);

        return await response.Content.ReadFromJsonAsync<PagedResult<TvSeriesListViewModel>>()
               ?? EmptyPaged<TvSeriesListViewModel>(page, pageSize);
    }

    // =========================
    // Unwatched TV Series (Watchlist)
    // =========================
    public async Task<PagedResult<TvSeriesListViewModel>> GetUnwatchedTvSeriesAsync(int page = 1, int pageSize = 24, string? search = null)
    {
        var http = ApiAuthed();
        var url = BuildPagedUrl("api/tmdb/tv/unwatched", page, pageSize, search);
        var response = await http.GetAsync(url);
        if (!response.IsSuccessStatusCode)
            return EmptyPaged<TvSeriesListViewModel>(page, pageSize);

        return await response.Content.ReadFromJsonAsync<PagedResult<TvSeriesListViewModel>>()
               ?? EmptyPaged<TvSeriesListViewModel>(page, pageSize);
    }

    // =========================
    // Get TV Watch Status
    // =========================
    public async Task<TvWatchStatusViewModel> GetTvWatchStatusAsync(int tmdbId)
    {
        var http = ApiAuthed();
        var response = await http.GetAsync($"api/tmdb/tv/{tmdbId}/watch-status");
        if (!response.IsSuccessStatusCode)
            return new TvWatchStatusViewModel();

        return await response.Content.ReadFromJsonAsync<TvWatchStatusViewModel>()
               ?? new TvWatchStatusViewModel();
    }

    // =========================
    // Mark Season As Watched
    // =========================
    public async Task<bool> MarkSeasonAsWatchedAsync(int tmdbId, int seasonNumber, bool watched)
    {
        var http = ApiAuthed();
        var result = await http.PostAsJsonAsync("api/tmdb/tv/mark-season",
            new { TmdbId = tmdbId, SeasonNumber = seasonNumber, Watched = watched });
        return result.IsSuccessStatusCode;
    }

    // =========================
    // DTO for Search
    // =========================
    private class SearchTvDto
    {
        public int TmdbId { get; set; }

        // TMDb bruker 'name' for TV-serier
        public string Name { get; set; } = string.Empty;

        // 'first_air_date' fra TMDb
        public string? FirstAirDate { get; set; }

        // vote_average fra TMDb
        public double? VoteAverage { get; set; }

        public string? PosterPath { get; set; }

        // Helper for å bygge full URL
        public string PosterUrl => !string.IsNullOrEmpty(PosterPath)
            ? $"https://image.tmdb.org/t/p/w500{PosterPath}"
            : "https://via.placeholder.com/300x450?text=No+Image";

        // Helper for å hente year
        public int? Year
        {
            get
            {
                if (!string.IsNullOrEmpty(FirstAirDate) && DateTime.TryParse(FirstAirDate, out var dt))
                    return dt.Year;
                return null;
            }
        }
    }

    // =========================
    // Get Season Episodes
    // =========================
    public async Task<SeasonEpisodeViewModel?> GetSeasonEpisodesAsync(int tmdbId, int seasonNumber)
    {
        var http = ApiAuthed();
        var response = await http.GetAsync($"api/tmdb/tv/{tmdbId}/season/{seasonNumber}/episodes");
        if (!response.IsSuccessStatusCode)
            return null;

        return await response.Content.ReadFromJsonAsync<SeasonEpisodeViewModel>();
    }

    // =========================
    // Mark Episode As Watched
    // =========================
    public async Task<bool> MarkEpisodeAsWatchedAsync(int episodeId, bool watched, int mediaItemId)
    {
        var http = ApiAuthed();
        var result = await http.PostAsJsonAsync("api/tmdb/tv/mark-episode",
            new { EpisodeId = episodeId, Watched = watched, MediaItemId = mediaItemId });
        return result.IsSuccessStatusCode;
    }

    // DTO for Import response
    private class ImportTvResponse
    {
        public int MediaItemId { get; set; }
        public bool AlreadyExisted { get; set; }
        public bool AlreadyInWatchlist { get; set; }
    }

    // =========================
    // Helpers
    // =========================
    private static string BuildPagedUrl(string baseUrl, int page, int pageSize, string? search)
    {
        var parts = new List<string> { $"page={page}", $"pageSize={pageSize}" };
        if (!string.IsNullOrWhiteSpace(search))
            parts.Add($"search={Uri.EscapeDataString(search)}");
        return $"{baseUrl}?{string.Join("&", parts)}";
    }

    private static PagedResult<T> EmptyPaged<T>(int page, int pageSize) =>
        new() { Items = [], Total = 0, Page = page, PageSize = pageSize };
}

