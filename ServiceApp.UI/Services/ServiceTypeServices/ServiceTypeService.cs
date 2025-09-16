using ServiceApp.UI.Models;
using System.Net.Http.Json;

namespace ServiceApp.UI.Services.ServiceTypeServices;

public class ServiceTypeService(IHttpClientFactory clients) : IServiceTypeService
{
    private readonly IHttpClientFactory _clients = clients;
    private HttpClient ApiAuthed() => _clients.CreateClient("ApiAuthed");

    public async Task<List<ServiceTypeModel>> GetServiceTypesAsync()
    {
        var http = ApiAuthed();
        return await http.GetFromJsonAsync<List<ServiceTypeModel>>("api/service-type") ?? [];
    }

    public async Task<ServiceTypeModel?> GetServiceTypeByIdAsync(int id)
    {
        var http = ApiAuthed();
        return await http.GetFromJsonAsync<ServiceTypeModel>($"api/service-type/{id}");
    }

    public async Task CreateServiceTypeAsync(ServiceTypeModel serviceType)
    {
        var http = ApiAuthed();
        await http.PostAsJsonAsync("api/service-type", serviceType);
    }

    public async Task UpdateServiceTypeAsync(ServiceTypeModel serviceType)
    {
        var http = ApiAuthed();
        await http.PutAsJsonAsync($"api/service-type/{serviceType.Id}", serviceType);
    }

    public async Task<bool> DeleteServiceTypeAsync(int id)
    {
        var http = ApiAuthed();
        var resp = await http.DeleteAsync($"api/service-type/{id}");
        return resp.IsSuccessStatusCode;
    }
}
