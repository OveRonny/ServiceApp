using ServiceApp.UI.Models;

namespace ServiceApp.UI.Services.MovieServices;

public class MovieSearchState
{
    public string Query { get; set; } = "";
    public List<MovieSearchViewModel.MovieResult> Results { get; set; } = new();
}
