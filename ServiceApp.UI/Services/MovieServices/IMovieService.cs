using ServiceApp.UI.Models;

namespace ServiceApp.UI.Services.MovieServices;

public interface IMovieService
{
    Task<List<MovieFullDtoModel>> GetAllMoviesWatchedAsync(int page = 1, int pageSize = 50);

    Task<PagedResult<MovieFullDtoModel>> GetFilteredWatchedMoviesAsync(int page = 1, int pageSize = 50, string? searchQuery = null, string? genre = null);
    Task<PagedResult<MovieFullDtoModel>> GetFilteredUnWatchedMoviesAsync(int page = 1, int pageSize = 50, string? searchQuery = null, string? genre = null);
    Task<WatchedMovieDetailsModel> GetWatchedMovieDetails(int mediaId);
    Task AddMovieToWatchList(int tmdbId);
    Task<List<MovieSearchViewModel.MovieResult>> SearchMoviesAsync(string query);
    Task<MovieDetailsViewModel?> GetMovieDetailsAsync(int mediaId);
    Task<MovieDetailsViewModel?> GetMovieDetailsFromTmdbAsync(int tmdbId);
    Task<Guid?> ImportMovieAsync(int tmdbId);
    Task<bool> MarkMovieAsWatchedAsync(
        int TmdbId,
        bool markAsWatched = false,
        DateTime? Date = null,
        bool? Liked = null,
        int? Rating = null);

}
