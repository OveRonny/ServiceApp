using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using serviceApp.Server.Entities.StatensVegvesen.Models;

namespace serviceApp.Server.Features.StatensVegvesen.Api;

public class ApiStaten
{
    private readonly HttpClient _httpClient;

    public ApiStaten(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<KjoretoyDataList> GetKjoretoyDataAsync(string regNr)
    {
        var response = await _httpClient.GetAsync($"?kjennemerke={regNr}");
        response.EnsureSuccessStatusCode();
        var responseBody = await response.Content.ReadAsStringAsync();
        var kjoretoyList = JsonSerializer.Deserialize<KjoretoyDataList>(responseBody);
        if (kjoretoyList == null)
            throw new Exception("Kunne ikke deserialisere JSON");
        return kjoretoyList;
    }
}
