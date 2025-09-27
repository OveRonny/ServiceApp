
using ServiceApp.UI.Models;
using System.Net.Http.Json;

namespace ServiceApp.UI.Services.StatensVegvesenServices;

public class StatensVegvesenService(IHttpClientFactory clients) : IStatensVegvesenService
{
    private readonly IHttpClientFactory _clients = clients;

    private HttpClient ApiAuthed() => _clients.CreateClient("ApiAuthed");

    public async Task<StatensVegvesenModel> GetVehicleInfo(string registrationNumber)
    {
        var http = ApiAuthed();
        var response = await http.GetFromJsonAsync<StatensVegvesenResponse>($"api/kjoretoydata/{registrationNumber}");
        return response?.Data ?? new StatensVegvesenModel();
    }

    public class StatensVegvesenResponse
    {
        public StatensVegvesenModel? Data { get; set; }
    }
}
