using Microsoft.AspNetCore.Components.Forms;
using ServiceApp.UI.Models;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

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

    public async Task<bool> CreateServiceRecordWithImageAsync(ServiceRecordModel record, IBrowserFile? file, CancellationToken ct = default)
    {
        var http = ApiAuthed();
        using var content = new MultipartFormDataContent();

        // Add the ServiceRecord command as JSON
        var recordJson = JsonSerializer.Serialize(record);
        content.Add(new StringContent(recordJson, Encoding.UTF8, "application/json"), "command");

        // Add the image file if present
        if (file != null)
        {
            var stream = file.OpenReadStream(maxAllowedSize: 10 * 1024 * 1024);
            content.Add(new StreamContent(stream)
            {
                Headers = { ContentType = new MediaTypeHeaderValue(file.ContentType) }
            }, "file", file.Name);
        }

        var resp = await http.PostAsync("/api/service-record/with-image", content, ct);
        return resp.IsSuccessStatusCode;
    }

    public async Task<List<int>?> GetServiceRecordImageIdsAsync(int serviceRecordId, CancellationToken ct = default)
    {
        var http = ApiAuthed();
        return await http.GetFromJsonAsync<List<int>>($"/api/service-record/{serviceRecordId}/images", ct);
    }

    public async Task<bool> UpdateServiceRecordWithImageAsync(ServiceRecordModel record, IBrowserFile? file)
    {
        var http = ApiAuthed();
        using var content = new MultipartFormDataContent();
        var json = JsonSerializer.Serialize(record);
        content.Add(new StringContent(json, Encoding.UTF8, "application/json"), "command");

        if (file != null)
        {
            var stream = file.OpenReadStream();
            content.Add(new StreamContent(stream), "file", file.Name);
        }

        var response = await http.PostAsync("/api/service-record/update-with-image", content);
        return response.IsSuccessStatusCode;
    }

    public async Task<string?> GetImageSasUrlAsync(int imageId)
    {
        var http = ApiAuthed();
        // Adjust the endpoint as needed
        var response = await http.GetAsync($"/api/images/{imageId}/sas");
        if (response.IsSuccessStatusCode)
        {
            var result = await response.Content.ReadFromJsonAsync<SasUrlResponse>();
            return result?.Url;
        }
        return null;
    }

    public async Task<Dictionary<int, string>> GetImageSasUrlsAsync(IEnumerable<int> imageIds)
    {
        var result = new Dictionary<int, string>();
        foreach (var id in imageIds)
        {
            var sasUrl = await GetImageSasUrlAsync(id);
            if (!string.IsNullOrEmpty(sasUrl))
                result[id] = sasUrl;
        }
        return result;
    }

    public class SasUrlResponse
    {
        public string? Url { get; set; }
    }
}


