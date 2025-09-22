using ServiceApp.UI.Models;
using System.Net.Http.Json;

namespace ServiceApp.UI.Services.ServiceRecordServices;

public class ServiceRecordService(IHttpClientFactory clients) : IServiceRecordService
{
    private readonly IHttpClientFactory _clients = clients;

    private HttpClient ApiAuthed() => _clients.CreateClient("ApiAuthed");

    public async Task<List<ServiceRecordModel>> GetServiceRecordsAsync(int vehicleId)
    {
        var http = ApiAuthed();
        var result = await http.GetFromJsonAsync<List<ServiceRecordModel>>($"api/service-record/vehicle/{vehicleId}");
        if (result == null) throw new Exception("Failed to load service records");
        return result;
    }

    public async Task<ServiceRecordModel> GetServiceRecordByIdAsync(int id)
    {
        var http = ApiAuthed();
        var result = await http.GetFromJsonAsync<ServiceRecordModel>($"api/service-record/{id}");
        if (result == null) throw new Exception("Failed to load service record");
        return result;
    }

    public async Task CreateServiceRecordAsync(ServiceRecordModel model)
    {
        var http = ApiAuthed();

        // Build UsedParts from the multi-row UI
        var usedParts = (model.UsedParts ?? new())
            .Where(p => p.VehicleInventoryId is int && p.Quantity > 0)
            // If your API expects int quantities, cast here. Remove the cast if server accepts decimal.
            .Select(p => new { VehicleInventoryId = p.VehicleInventoryId!.Value, Quantity = (int)Math.Round(p.Quantity) })
            .ToList();

        // Fallback for legacy single-select (optional)
        if (usedParts.Count == 0 && model.VehicleInventoryId is int invId && model.QuantityUsed > 0)
        {
            usedParts.Add(new { VehicleInventoryId = invId, Quantity = (int)Math.Round(model.QuantityUsed) });
        }

        var command = new
        {
            model.VehicleId,
            model.ServiceTypeId,
            model.Description,
            model.Cost,
            model.Mileage,
            model.Hours,               // API command has Hours (nullable)
            model.ServiceCompanyId,
            UsedParts = usedParts
        };

        var resp = await http.PostAsJsonAsync("api/service-record", command);
        resp.EnsureSuccessStatusCode();
    }

    public async Task UpdateServiceRecordAsync(ServiceRecordModel model)
    {
        var http = ApiAuthed();
        // NOTE: If your server PUT endpoint does not handle UsedParts yet,
        // this will not update parts. You’ll need a server-side update handler
        // similar to create that diffs or replaces parts.
        var result = await http.PutAsJsonAsync($"api/service-record/{model.Id}", model);
        result.EnsureSuccessStatusCode();
    }

    public async Task<bool> DeleteServiceRecordAsync(int id)
    {
        var http = ApiAuthed();
        var result = await http.DeleteAsync($"api/service-record/{id}");
        return result.IsSuccessStatusCode;
    }
}
