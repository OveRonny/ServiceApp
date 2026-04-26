using ServiceApp.UI.Models;

namespace ServiceApp.UI.Services.TvServices;

public interface ITvservice
{
    Task<List<TvSearchViewModel.TvResult>> SearchTvAsync(string query);
    Task<TvDetailsViewModel?> GetTvDetailsAsync(int tmdbId);
    Task<(bool Success, bool AlreadyInWatchlist)> ImportTvAsync(int tmdbId, StreamingService? streamingService = null);
    Task<bool> UpdateStreamingServiceAsync(int tmdbId, StreamingService? streamingService);
    Task<PagedResult<TvSeriesListViewModel>> GetWatchedTvSeriesAsync(int page = 1, int pageSize = 24, string? search = null);
    Task<PagedResult<TvSeriesListViewModel>> GetUnwatchedTvSeriesAsync(int page = 1, int pageSize = 24, string? search = null);
    Task<TvWatchStatusViewModel> GetTvWatchStatusAsync(int tmdbId);
    Task<bool> MarkSeasonAsWatchedAsync(int tmdbId, int seasonNumber, bool watched);
    Task<SeasonEpisodeViewModel?> GetSeasonEpisodesAsync(int tmdbId, int seasonNumber);
    Task<bool> MarkEpisodeAsWatchedAsync(int episodeId, bool watched, int mediaItemId);
    Task<bool> UpdateWatchCommentAsync(int tmdbId, string mediaType, string? comment);
}

