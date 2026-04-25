namespace serviceApp.Server.Features.Tmdb;

using System.Net;
using System.Net.Http.Json;

public class TmdbClient
{
    private readonly HttpClient _http;
    private readonly string _apiKey;

    public TmdbClient(HttpClient http, IConfiguration config)
    {
        _http = http;
        _apiKey = config["TMDB:ApiKey"]
                  ?? throw new InvalidOperationException("TMDB ApiKey missing");
    }

    // =========================
    // SEARCH (movie + tv)
    // =========================
    public async Task<TmdbPagedResponse<TmdbSearchResultDto>> SearchMultiAsync(string query)
    {
        var url =
            $"search/multi?query={Uri.EscapeDataString(query)}&api_key={_apiKey}";

        return await _http.GetFromJsonAsync<
            TmdbPagedResponse<TmdbSearchResultDto>>(url)
            ?? new TmdbPagedResponse<TmdbSearchResultDto>();
    }

    // =========================
    // SEARCH MOVIE ONLY
    // =========================
    public async Task<TmdbPagedResponse<TmdbSearchResultDto>> SearchMovieAsync(string query)
    {
        var url =
            $"search/movie?query={Uri.EscapeDataString(query)}&api_key={_apiKey}";

        return await _http.GetFromJsonAsync<
            TmdbPagedResponse<TmdbSearchResultDto>>(url)
            ?? new TmdbPagedResponse<TmdbSearchResultDto>();
    }

    // =========================
    // SEARCH TV ONLY
    // =========================
    public async Task<TmdbPagedResponse<TmdbSearchResultDto>> SearchTvAsync(string query)
    {
        var url =
            $"search/tv?query={Uri.EscapeDataString(query)}&api_key={_apiKey}";

        return await _http.GetFromJsonAsync<
            TmdbPagedResponse<TmdbSearchResultDto>>(url)
            ?? new TmdbPagedResponse<TmdbSearchResultDto>();
    }

    // =========================
    // MOVIE DETAILS
    // =========================
    public async Task<TmdbMovieDetailsDto?> GetMovieAsync(int tmdbId)
    {
        var url =
            $"movie/{tmdbId}?api_key={_apiKey}&append_to_response=external_ids";

        var response = await _http.GetAsync(url);

        if (response.StatusCode == HttpStatusCode.NotFound)
            return null;

        if (!response.IsSuccessStatusCode)
            throw new Exception($"TMDb error: {response.StatusCode}");

        return await response.Content.ReadFromJsonAsync<TmdbMovieDetailsDto>();
    }

    // =========================
    // TV DETAILS
    // =========================
    public async Task<TmdbTvDetailsDto?> GetTvDetailsAsync(int tmdbId)
    {
        var url =
            $"tv/{tmdbId}?api_key={_apiKey}&append_to_response=external_ids";

        var response = await _http.GetAsync(url);

        if (response.StatusCode == HttpStatusCode.NotFound)
            return null;

        if (!response.IsSuccessStatusCode)
            throw new Exception($"TMDb error: {response.StatusCode}");

        return await response.Content.ReadFromJsonAsync<TmdbTvDetailsDto>();
    }

    // =========================
    // SEASON EPISODES
    // =========================
    public async Task<TmdbSeasonApiDto?> GetSeasonAsync(int tvTmdbId, int seasonNumber)
    {
        var url = $"tv/{tvTmdbId}/season/{seasonNumber}?api_key={_apiKey}";
        var response = await _http.GetAsync(url);

        if (response.StatusCode == HttpStatusCode.NotFound)
            return null;

        if (!response.IsSuccessStatusCode)
            throw new Exception($"TMDb error: {response.StatusCode}");

        return await response.Content.ReadFromJsonAsync<TmdbSeasonApiDto>();
    }
}

