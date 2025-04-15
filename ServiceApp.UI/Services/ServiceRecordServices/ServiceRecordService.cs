using ServiceApp.UI.Models;
using System.Net.Http.Json;

namespace ServiceApp.UI.Services.ServiceRecordServices;

public class ServiceRecordService(HttpClient http) : IServiceRecordService
{
    private readonly HttpClient http = http;

    public async Task<List<ServiceRecordModel>> GetServiceRecordsAsync()
    {
        var result = await http.GetFromJsonAsync<List<ServiceRecordModel>>("api/service-record");
        if (result == null)
        {
            throw new Exception("Failed to load service records");
        }
        return result;
    }

    public async Task<ServiceRecordModel> GetServiceRecordByIdAsync(int id)
    {
        var result = await http.GetFromJsonAsync<ServiceRecordModel>($"api/service-record/{id}");
        if (result == null)
        {
            throw new Exception($"Failed to load service record with id {id}");
        }
        return result;
    }

    public async Task<ServiceRecordModel> CreateServiceRecordAsync(ServiceRecordModel serviceRecord)
    {
        var result = await http.PostAsJsonAsync("api/service-record", serviceRecord);

        if (result.IsSuccessStatusCode)
        {
            var newServiceRecord = await result.Content.ReadFromJsonAsync<ServiceRecordModel>();
            if (newServiceRecord != null)
            {
                return newServiceRecord;
            }
            else
            {
                throw new Exception("Failed to deserialize the service record");
            }
        }
        else
        {
            throw new Exception("Failed to create service record");
        }
    }

    public async Task<ServiceRecordModel> UpdateServiceRecordAsync(ServiceRecordModel serviceRecord)
    {
        var result = await http.PutAsJsonAsync($"api/service-record/{serviceRecord.Id}", serviceRecord);
        if (result.IsSuccessStatusCode)
        {
            var updatedServiceRecord = await result.Content.ReadFromJsonAsync<ServiceRecordModel>();
            if (updatedServiceRecord != null)
            {
                return updatedServiceRecord;
            }
            else
            {
                throw new Exception("Failed to deserialize the service record");
            }
        }
        else
        {
            throw new Exception("Failed to update service record");
        }
    }

    public async Task<bool> DeleteServiceRecordAsync(int id)
    {
        var result = await http.DeleteAsync($"api/service-record/{id}");
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
