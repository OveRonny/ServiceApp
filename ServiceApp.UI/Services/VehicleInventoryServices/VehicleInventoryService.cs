using ServiceApp.UI.Models;
using System.Net.Http.Json;

namespace ServiceApp.UI.Services.VehicleInventoryServices;

public class VehicleInventoryService(IHttpClientFactory clients) : IVehicleInventoryService
{
    private readonly IHttpClientFactory _clients = clients;
    private HttpClient ApiAuthed() => _clients.CreateClient("ApiAuthed");

    public Task<List<VehicleInventoryModel>> GetVehicleInventoryAsync(int vehicleId)
         => GetVehicleInventoryAsync(vehicleId, includeZero: true);

    public async Task<List<VehicleInventoryModel>> GetVehicleInventoryAsync(int vehicleId, bool includeZero)
    {
        var http = ApiAuthed();
        var url = $"api/vehicle-inventory/vehicle/{vehicleId}?includeZero={(includeZero ? "true" : "false")}";
        return await http.GetFromJsonAsync<List<VehicleInventoryModel>>(url) ?? [];
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

    public async Task AdjustInventoryAsync(
        int id,
        int quantityDelta,
        string partName,
        string description,
        decimal cost,
        int vehicleId,
        int supplierId,
        int? reorderThreshold)
        
    {
        var http = ApiAuthed();

        var command = new
        {
            Id = id,
            PartName = partName,
            Cost = cost,
            Description = description,
            VehicleId = vehicleId,
            SupplierId = supplierId,
            QuantityInStock = (decimal?)null, // not used when delta is provided
            ReorderThreshold = reorderThreshold,         
            QuantityDelta = quantityDelta     // key field to adjust stock
        };

        var resp = await http.PutAsJsonAsync($"api/vehicle-inventory/{id}", command);
        resp.EnsureSuccessStatusCode();
    }


}
