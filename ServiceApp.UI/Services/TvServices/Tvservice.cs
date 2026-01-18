
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
        var response = await http.GetFromJsonAsync<TvDetailsApiDto>(
            $"api/tmdb/tv/{tmdbId}");

        return response == null ? null : TestMapper.Map(response);
    }


    // =========================
    // Import TV
    // =========================
    public async Task<Guid?> ImportTvAsync(int tmdbId)
    {
        var http = ApiAuthed();
        var result = await http.PostAsJsonAsync("api/tmdb/import/tv", new { TmdbId = tmdbId });

        if (!result.IsSuccessStatusCode)
            return null;

        var response = await result.Content.ReadFromJsonAsync<ImportTvResponse>();
        return response?.MediaItemId;
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

    // DTO for Import response
    private class ImportTvResponse
    {
        public Guid MediaItemId { get; set; }
    }
}

