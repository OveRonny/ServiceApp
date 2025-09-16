using ServiceApp.UI.Models;
using System.Net.Http.Json;

namespace ServiceApp.UI.Services.VehicleInventoryServices;

public class VehicleInventoryService(IHttpClientFactory clients) : IVehicleInventoryService
{
    private readonly IHttpClientFactory _clients = clients;
    private HttpClient ApiAuthed() => _clients.CreateClient("ApiAuthed");

    public async Task<List<VehicleInventoryModel>> GetVehicleInventoryAsync(int vehicleId)
    {
        var http = ApiAuthed();
        return await http.GetFromJsonAsync<List<VehicleInventoryModel>>($"api/vehicle-inventory/vehicle/{vehicleId}") ?? [];
    }

    public async Task<VehicleInventoryModel?> GetVehicleInventoryByIdAsync(int id)
    {
        var http = ApiAuthed();
        return await http.GetFromJsonAsync<VehicleInventoryModel>($"api/vehicle-inventory/{id}");
    }

    public async Task CreateVehicleInventoryAsync(VehicleInventoryModel vehicleInventory)
    {
        var http = ApiAuthed();
        await http.PostAsJsonAsync("api/vehicle-inventory", vehicleInventory);
    }

    public async Task UpdateVehicleInventoryAsync(VehicleInventoryModel vehicleInventory)
    {
        var http = ApiAuthed();
        await http.PutAsJsonAsync($"api/vehicle-inventory/{vehicleInventory.Id}", vehicleInventory);
    }

    public async Task DeleteVehicleInventoryAsync(int id)
    {
        var http = ApiAuthed();
        await http.DeleteAsync($"api/vehicle-inventory/{id}");
    }
}
