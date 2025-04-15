using ServiceApp.UI.Models;
using System.Net.Http.Json;

namespace ServiceApp.UI.Services.ServiceCompanyServices;

public class ServiceCompanyService(HttpClient http) : IServiceCompanyService
{
    private readonly HttpClient http = http;

    public async Task<List<ServiceCompanyModel>> GetAllServiceCompaniesAsync()
    {
        var result = await http.GetFromJsonAsync<List<ServiceCompanyModel>>("api/service-company");
        return result ?? new List<ServiceCompanyModel>();
    }

    public async Task<ServiceCompanyModel?> GetServiceCompanyByIdAsync(int id)
    {
        var result = await http.GetFromJsonAsync<ServiceCompanyModel>($"api/service-company/{id}");
        return result;
    }

    public async Task<ServiceCompanyModel> AddServiceCompanyAsync(ServiceCompanyModel serviceCompany)
    {
        var result = await http.PostAsJsonAsync("api/service-company", serviceCompany);
        if (result.IsSuccessStatusCode)
        {
            var newServiceCompany = await result.Content.ReadFromJsonAsync<ServiceCompanyModel>();
            if (newServiceCompany != null)
            {
                return newServiceCompany;
            }
            else
            {
                throw new Exception("Failed to deserialize the service company");
            }
        }
        else
        {
            throw new Exception("Failed to add service company");
        }
    }

    public async Task<ServiceCompanyModel> UpdateServiceCompanyAsync(ServiceCompanyModel serviceCompany)
    {
        var result = await http.PutAsJsonAsync($"api/service-company/{serviceCompany.Id}", serviceCompany);
        if (result.IsSuccessStatusCode)
        {
            var updatedServiceCompany = await result.Content.ReadFromJsonAsync<ServiceCompanyModel>();
            if (updatedServiceCompany != null)
            {
                return updatedServiceCompany;
            }
            else
            {
                throw new Exception("Failed to deserialize the service company");
            }
        }
        else
        {
            throw new Exception("Failed to update service company");
        }
    }

    public async Task<bool> DeleteServiceCompanyAsync(int id)
    {
        var result = await http.DeleteAsync($"api/service-company/{id}");
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
