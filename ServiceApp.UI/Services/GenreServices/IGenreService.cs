using ServiceApp.UI.Models;

namespace ServiceApp.UI.Services.GenreServices;

public interface IGenreService
{
    Task<List<GenreDtoModel>> GetAllGenresAsync();
}
