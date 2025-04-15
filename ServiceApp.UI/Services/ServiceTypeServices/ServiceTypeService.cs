using ServiceApp.UI.Models;
using System.Net.Http.Json;

namespace ServiceApp.UI.Services.ServiceTypeServices;

public class ServiceTypeService(HttpClient http) : IServiceTypeService
{
    private readonly HttpClient http = http;

    public async Task<List<ServiceTypeModel>> GetServiceTypesAsync()
    {
        var result = await http.GetFromJsonAsync<List<ServiceTypeModel>>("api/service-type");
        if (result == null)
        {
            throw new Exception("Failed to load service types");
        }
        return result;
    }

    public async Task<ServiceTypeModel?> GetServiceTypeByIdAsync(int id)
    {
        var result = await http.GetFromJsonAsync<ServiceTypeModel>($"api/service-type/{id}");
        if (result == null)
        {
            return null;
        }
        return result;
    }

    public async Task<ServiceTypeModel> CreateServiceTypeAsync(ServiceTypeModel serviceType)
    {
        var result = await http.PostAsJsonAsync("api/service-type", serviceType);
        if (result.IsSuccessStatusCode)
        {
            var createdServiceType = await result.Content.ReadFromJsonAsync<ServiceTypeModel>();
            if (createdServiceType == null)
            {
                throw new Exception("Failed to create service type");
            }
            return createdServiceType;
        }
        else
        {
            throw new Exception("Failed to create service type");
        }
    }

    public async Task<ServiceTypeModel> UpdateServiceTypeAsync(ServiceTypeModel serviceType)
    {
        var result = await http.PutAsJsonAsync($"api/service-type/{serviceType.Id}", serviceType);
        if (result.IsSuccessStatusCode)
        {
            var updatedServiceType = await result.Content.ReadFromJsonAsync<ServiceTypeModel>();
            if (updatedServiceType == null)
            {
                throw new Exception("Failed to update service type");
            }
            return updatedServiceType;
        }
        else
        {
            throw new Exception("Failed to update service type");
        }
    }

    public async Task<bool> DeleteServiceTypeAsync(int id)
    {
        var result = await http.DeleteAsync($"api/service-type/{id}");
        if (result.IsSuccessStatusCode)
        {
            return true;
        }
        else
        {
            throw new Exception("Failed to delete service type");
        }
    }
}
