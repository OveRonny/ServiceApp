using ServiceApp.UI.Models;
using System.Net.Http.Json;

namespace ServiceApp.UI.Services.VehicleInventoryServices;

public class VehicleInventoryService(HttpClient http) : IVehicleInventoryService
{
    private readonly HttpClient http = http;

    public async Task<List<VehicleInventoryModel>> GetVehicleInventoryAsync(int vehicleId)
    {
        var result = await http.GetFromJsonAsync<List<VehicleInventoryModel>>($"api/vehicle-inventory/vehicle/{vehicleId}");
        return result ?? new List<VehicleInventoryModel>();
    }

    public async Task<VehicleInventoryModel> CreateVehicleInventoryAsync(VehicleInventoryModel vehicleInventory)
    {
        var result = await http.PostAsJsonAsync("api/vehicle-inventory", vehicleInventory);
        if (result.IsSuccessStatusCode)
        {
            var vehicleInventoryResult = await result.Content.ReadFromJsonAsync<VehicleInventoryModel>();
            return vehicleInventoryResult ?? new VehicleInventoryModel();
        }
        else
        {
            throw new Exception("Failed to create vehicle inventory");
        }

    }
    public async Task<VehicleInventoryModel> UpdateVehicleInventoryAsync(VehicleInventoryModel vehicleInventory)
    {
        var result = await http.PutAsJsonAsync($"api/vehicle-inventory/{vehicleInventory.Id}", vehicleInventory);
        if (result.IsSuccessStatusCode)
        {
            var vehicleInventoryResult = await result.Content.ReadFromJsonAsync<VehicleInventoryModel>();
            return vehicleInventoryResult ?? new VehicleInventoryModel();
        }
        else
        {
            throw new Exception("Failed to update vehicle inventory");
        }
    }

    public async Task<bool> DeleteVehicleInventoryAsync(int id)
    {
        var result = await http.DeleteAsync($"api/vehicle-inventory/{id}");
        if (result.IsSuccessStatusCode)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
