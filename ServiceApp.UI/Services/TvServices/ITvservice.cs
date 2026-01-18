using ServiceApp.UI.Models;

namespace ServiceApp.UI.Services.TvServices;

public interface ITvservice
{
    Task<List<TvSearchViewModel.TvResult>> SearchTvAsync(string query);
    Task<TvDetailsViewModel?> GetTvDetailsAsync(int tmdbId);
    Task<Guid?> ImportTvAsync(int tmdbId);
}
