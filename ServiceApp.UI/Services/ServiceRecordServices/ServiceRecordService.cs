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
        if (result == null)
        {
            throw new Exception("Failed to load service records");
        }
        return result;

    }

    public async Task<ServiceRecordModel> GetServiceRecordByIdAsync(int id)
    {
        var http = ApiAuthed();
        var result = await http.GetFromJsonAsync<ServiceRecordModel>($"api/service-record/{id}");
        if (result == null)
        {
            throw new Exception("Failed to load service record");
        }
        return result;
    }

    public async Task CreateServiceRecordAsync(ServiceRecordModel serviceRecord)
    {
        var http = ApiAuthed();
        var result = await http.PostAsJsonAsync("api/service-record", serviceRecord);
        result.EnsureSuccessStatusCode();
    }

    public async Task UpdateServiceRecordAsync(ServiceRecordModel serviceRecord)
    {
        var http = ApiAuthed();
        var result = await http.PutAsJsonAsync($"api/service-record/{serviceRecord.Id}", serviceRecord);
        result.EnsureSuccessStatusCode();
    }

    public async Task<bool> DeleteServiceRecordAsync(int id)
    {
        var http = ApiAuthed();
        var result = await http.DeleteAsync($"api/service-record/{id}");
        return result.IsSuccessStatusCode;
    }
}
