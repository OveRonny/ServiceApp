using ServiceApp.UI.Models;

namespace ServiceApp.UI.Services.TvServices;

public class TvSearchState
{
    public string Query { get; set; } = "";
    public List<TvSearchViewModel.TvResult> Results { get; set; } = new();
}
