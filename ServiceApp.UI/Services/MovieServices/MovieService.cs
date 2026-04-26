using ServiceApp.UI.Models;
using System.Net;
using System.Net.Http.Json;

namespace ServiceApp.UI.Services.MovieServices;

public class MovieService(IHttpClientFactory http) : IMovieService
{
    private readonly IHttpClientFactory http = http;

    private HttpClient ApiAuthed() => http.CreateClient("ApiAuthed");

    public async Task<List<MovieFullDtoModel>> GetAllMoviesWatchedAsync(int page = 1, int pageSize = 50)
    {
        var http = ApiAuthed();
        var url = $"api/tmdb/movies/watched?page={page}&pageSize={pageSize}";
        var result = await http.GetFromJsonAsync<List<MovieFullDtoModel>>(url);

        return result ?? new List<MovieFullDtoModel>();
    }

    public async Task<PagedResult<MovieFullDtoModel>> GetFilteredWatchedMoviesAsync(
    int page = 1,
    int pageSize = 50,
    string? searchQuery = null,
    string? genre = null)

    {
        var http = ApiAuthed(); // Authenticated HttpClient

        // Build query string
        var queryParams = new List<string>
        {
            $"page={page}",
            $"pageSize={pageSize}"
        };

        if (!string.IsNullOrWhiteSpace(searchQuery))
            queryParams.Add($"search={Uri.EscapeDataString(searchQuery)}");

        if (!string.IsNullOrWhiteSpace(genre))
            queryParams.Add($"genre={Uri.EscapeDataString(genre)}");

        var url = $"api/tmdb/movies/watched/filter?{string.Join("&", queryParams)}";

        try
        {
            var result = await http.GetFromJsonAsync<PagedResult<MovieFullDtoModel>>(url);

            return result ?? new PagedResult<MovieFullDtoModel>
            {
                Items = new List<MovieFullDtoModel>(),
                Total = 0,
                Page = page,
                PageSize = pageSize
            };
        }
        catch
        {
            return new PagedResult<MovieFullDtoModel>
            {
                Items = new List<MovieFullDtoModel>(),
                Total = 0,
                Page = page,
                PageSize = pageSize
            };
        }
    }

    public async Task<PagedResult<MovieFullDtoModel>> GetFilteredUnWatchedMoviesAsync(int page = 1, int pageSize = 50, string? searchQuery = null, string? genre = null)
    {
        var http = ApiAuthed(); // Authenticated HttpClient

        // Build query string
        var queryParams = new List<string>
        {
            $"page={page}",
            $"pageSize={pageSize}"
        };

        if (!string.IsNullOrWhiteSpace(searchQuery))
            queryParams.Add($"search={Uri.EscapeDataString(searchQuery)}");

        if (!string.IsNullOrWhiteSpace(genre))
            queryParams.Add($"genre={Uri.EscapeDataString(genre)}");

        var url = $"api/tmdb/movies/un-watched/filter?{string.Join("&", queryParams)}";

        try
        {
            var result = await http.GetFromJsonAsync<PagedResult<MovieFullDtoModel>>(url);

            return result ?? new PagedResult<MovieFullDtoModel>
            {
                Items = new List<MovieFullDtoModel>(),
                Total = 0,
                Page = page,
                PageSize = pageSize
            };
        }
        catch
        {
            return new PagedResult<MovieFullDtoModel>
            {
                Items = new List<MovieFullDtoModel>(),
                Total = 0,
                Page = page,
                PageSize = pageSize
            };
        }
    }

    public async Task<WatchedMovieDetailsModel> GetWatchedMovieDetails(int MediaId)
    {
        var http = ApiAuthed();
        var result = await http.GetFromJsonAsync<WatchedMovieDetailsModel>($"api/tmdb/movies/watched/details/{MediaId}");
        return result ?? new WatchedMovieDetailsModel();
    }


    public async Task<List<MovieSearchViewModel.MovieResult>> SearchMoviesAsync(string query)
    {
        var http = ApiAuthed();
        var url = $"api/tmdb/search?query={Uri.EscapeDataString(query)}";
        var response = await http.GetFromJsonAsync<List<SearchMovieDto>>(url);
        return response?.Select(r => new MovieSearchViewModel.MovieResult
        {
            TmdbId = r.TmdbId,
            Title = r.Title,
            MediaType = r.MediaType,
            PosterPath = r.PosterPath,
            Year = r.Year
        }).ToList() ?? new List<MovieSearchViewModel.MovieResult>();
    }

    public async Task<MovieDetailsViewModel?> GetMovieDetailsAsync(int tmdbId)
    {
        var http = ApiAuthed();
        var response = await http.GetAsync($"api/tmdb/movie/{tmdbId}");

        if (response.StatusCode == HttpStatusCode.NotFound)
            return null;

        if (response.StatusCode == HttpStatusCode.BadRequest)
            return null;

        if (!response.IsSuccessStatusCode)
            return null;

        return await response.Content.ReadFromJsonAsync<MovieDetailsViewModel>();
    }

    public async Task<Guid?> ImportMovieAsync(int tmdbId)
    {
        var http = ApiAuthed();
        var result = await http.PostAsJsonAsync("api/tmdb/import/movie", new { TmdbId = tmdbId });
        if (!result.IsSuccessStatusCode)
            return null;

        var response = await result.Content.ReadFromJsonAsync<ImportMovieResponse>();
        return response?.MediaItemId;
    }

    public record MarkMovieCommand(
        int TmdbId,
        DateTime? Date = null,
        bool? Liked = null,
        int? Rating = null
    );



    public async Task<bool> MarkMovieAsWatchedAsync(
       int TmdbId,
       bool markAsWatched = false,
       DateTime? Date = null,
       bool? Liked = null,
       int? Rating = null,
       string? Comment = null)
    {
        var http = ApiAuthed();
        var command = new
        {
            TmdbId,
            MarkAsWatched = markAsWatched,
            Date,
            Liked,
            Rating,
            Comment
        };

        var response = await http.PostAsJsonAsync("/api/movies/markaswatched", command);
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> UpdateWatchCommentAsync(int tmdbId, string mediaType, string? comment)
    {
        var http = ApiAuthed();
        var result = await http.PatchAsJsonAsync("api/tmdb/watchhistory/comment",
            new { TmdbId = tmdbId, MediaType = mediaType, Comment = comment });
        return result.IsSuccessStatusCode;
    }

    public async Task<bool> UpdateStreamingServiceAsync(int tmdbId, StreamingService? streamingService)
    {
        var http = ApiAuthed();
        var result = await http.PatchAsJsonAsync("api/tmdb/watchlist/streaming",
            new { TmdbId = tmdbId, StreamingService = (int?)streamingService, MediaType = "movie" });
        return result.IsSuccessStatusCode;
    }

    public async Task AddMovieToWatchList(int tmdbId, StreamingService? streamingService = null)
    {
        var http = ApiAuthed();
        await http.PostAsJsonAsync("api/tmdb/movies/watchlist", new { TmdbId = tmdbId, StreamingService = (int?)streamingService });
    }

    public Task<MovieDetailsViewModel?> GetMovieDetailsFromTmdbAsync(int tmdbId)
    {
        var http = ApiAuthed();
        return http.GetFromJsonAsync<MovieDetailsViewModel?>($"api/tmdb/movie/details-tmdb/{tmdbId}");
    }


}

// DTO for search
public class SearchMovieDto
{
    public int TmdbId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string MediaType { get; set; } = "movie";
    public string? PosterPath { get; set; }
    public int? Year { get; set; }
}

public class ImportMovieResponse
{
    public Guid MediaItemId { get; set; }
}
