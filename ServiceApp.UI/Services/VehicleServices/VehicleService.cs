using ServiceApp.UI.Models;
using System.Net.Http.Json;

namespace ServiceApp.UI.Services.VehicleServices;

public class VehicleService(IHttpClientFactory clients) : IVehicleService
{
    private readonly IHttpClientFactory _clients = clients;

    private HttpClient ApiAuthed() => _clients.CreateClient("ApiAuthed");

    public async Task<List<VehicleModel>> GetAllVehiclesAsync()
    {
        var http = ApiAuthed();
        return await http.GetFromJsonAsync<List<VehicleModel>>("api/vehicle") ?? [];
    }

    public async Task<VehicleModel?> GetVehicleByIdAsync(int id)
    {
        var http = ApiAuthed();
        return await http.GetFromJsonAsync<VehicleModel>($"api/vehicle/{id}");
    }

    public async Task<List<VehicleModel>> GetVehiclesByOwnerIdAsync(int ownerId)
    {
        var http = ApiAuthed();
        return await http.GetFromJsonAsync<List<VehicleModel>>($"api/vehicle/owner/{ownerId}") ?? [];
    }

    public async Task AddVehicleAsync(VehicleModel model)
    {
        var http = ApiAuthed();
        var resp = await http.PostAsJsonAsync("api/vehicle", model);
        resp.EnsureSuccessStatusCode();
    }

    public async Task UpdateVehicleAsync(VehicleModel model)
    {
        var http = ApiAuthed();
        var resp = await http.PutAsJsonAsync($"api/vehicle/{model.Id}", model);
        resp.EnsureSuccessStatusCode();
    }

    public async Task DeleteVehicleAsync(int id)
    {
        var http = ApiAuthed();
        var resp = await http.DeleteAsync($"api/vehicle/{id}");
        resp.EnsureSuccessStatusCode();
    }
}
