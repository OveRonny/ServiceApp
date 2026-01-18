
using ServiceApp.UI.Models;
using System.Net.Http.Json;

namespace ServiceApp.UI.Services.GenreServices;

public class GenreService(IHttpClientFactory http) : IGenreService
{
    private readonly IHttpClientFactory http = http;

    private HttpClient ApiAuthed() => http.CreateClient("ApiAuthed");

    public async Task<List<GenreDtoModel>> GetAllGenresAsync()
    {
        var http = ApiAuthed();
        var result = await http.GetFromJsonAsync<List<GenreDtoModel>>("/api/tmdb/genres");
        return result ?? new List<GenreDtoModel>();

    }
}
