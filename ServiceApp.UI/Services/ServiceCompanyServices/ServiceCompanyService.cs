using ServiceApp.UI.Models;
using System.Net.Http.Json;

namespace ServiceApp.UI.Services.ServiceCompanyServices;

public class ServiceCompanyService(IHttpClientFactory clients) : IServiceCompanyService
{
    private readonly IHttpClientFactory _clients = clients;

    private HttpClient ApiAuthed() => _clients.CreateClient("ApiAuthed");

    public async Task<List<ServiceCompanyModel>> GetAllServiceCompaniesAsync()
    {
        var http = ApiAuthed();
        return await http.GetFromJsonAsync<List<ServiceCompanyModel>>("api/service-company") ?? [];
    }

    public async Task<ServiceCompanyModel?> GetServiceCompanyByIdAsync(int id)
    {
        var http = ApiAuthed();
        return await http.GetFromJsonAsync<ServiceCompanyModel>($"api/service-company/{id}");
    }

    public async Task AddServiceCompanyAsync(ServiceCompanyModel serviceCompany)
    {
        var http = ApiAuthed();
        await http.PostAsJsonAsync("api/service-company", serviceCompany);
    }

    public async Task UpdateServiceCompanyAsync(ServiceCompanyModel serviceCompany)
    {
        var http = ApiAuthed();
        await http.PutAsJsonAsync($"api/service-company/{serviceCompany.Id}", serviceCompany);
    }

    public async Task<bool> DeleteServiceCompanyAsync(int id)
    {
        var http = ApiAuthed();
        var resp = await http.DeleteAsync($"api/service-company/{id}");
        return resp.IsSuccessStatusCode;
    }
}
