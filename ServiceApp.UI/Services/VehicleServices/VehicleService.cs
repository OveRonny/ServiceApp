using ServiceApp.UI.Models;
using System.Net.Http.Json;

namespace ServiceApp.UI.Services.VehicleServices;

public class VehicleService(HttpClient http) : IVehicleService
{
    private readonly HttpClient http = http;

    public async Task<List<VehicleModel>> GetAllVehiclesAsync()
    {
        var result = await http.GetFromJsonAsync<List<VehicleModel>>("api/vehicle");
        return result ?? new List<VehicleModel>();
    }

    public async Task<VehicleModel?> GetVehicleByIdAsync(int id)
    {
        var result = await http.GetFromJsonAsync<VehicleModel>($"api/vehicle/{id}");
        return result;
    }

    public async Task<VehicleModel> AddVehicleAsync(VehicleModel vehicle)
    {
        var result = await http.PostAsJsonAsync("api/vehicle", vehicle);
        if (result.IsSuccessStatusCode)
        {
            var newVehicle = await result.Content.ReadFromJsonAsync<VehicleModel>();
            if (newVehicle != null)
            {
                return newVehicle;
            }
            else
            {
                throw new Exception("Failed to deserialize the vehicle");
            }
        }
        else
        {
            throw new Exception("Failed to add vehicle");
        }

    }

    public async Task<VehicleModel> UpdateVehicleAsync(VehicleModel vehicle)
    {
        var result = await http.PutAsJsonAsync($"api/vehicle/{vehicle.Id}", vehicle);
        if (result.IsSuccessStatusCode)
        {
            var updatedVehicle = await result.Content.ReadFromJsonAsync<VehicleModel>();
            if (updatedVehicle != null)
            {
                return updatedVehicle;
            }
            else
            {
                throw new Exception("Failed to deserialize the vehicle");
            }
        }
        else
        {
            throw new Exception("Failed to update vehicle");
        }
    }

    public async Task<bool> DeleteVehicleAsync(int id)
    {
        var deleteVehicle = await http.DeleteAsync($"api/vehicle/{id}");
        if (deleteVehicle != null)
            return true;

        return false;
    }

}
