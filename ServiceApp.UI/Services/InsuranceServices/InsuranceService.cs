using ServiceApp.UI.Models;
using System.Net;
using System.Net.Http.Json;

namespace ServiceApp.UI.Services.InsuranceServices;

public class InsuranceService(IHttpClientFactory httpClient) : IInsuranceService
{
    private readonly IHttpClientFactory _clients = httpClient;

    private HttpClient ApiAuthed() => _clients.CreateClient("ApiAuthed");

    public async Task<List<InsuranseModel>> GetAllInsurances()
    {
        var http = ApiAuthed();
        return await http.GetFromJsonAsync<List<InsuranseModel>>("api/insurance") ?? [];
    }

    public Task<InsuranseModel?> GetInsuranceById(int id)
    {
        var http = ApiAuthed();
        return http.GetFromJsonAsync<InsuranseModel>($"api/insurance/{id}");
    }

    public async Task CreateInsurance(InsuranseModel insuranseModel)
    {
        var http = ApiAuthed();
        await http.PostAsJsonAsync("api/insurance", insuranseModel);
    }

    public async Task UpdateInsurance(InsuranseModel insuranseModel)
    {
        var http = ApiAuthed();
        await http.PutAsJsonAsync($"api/insurance/{insuranseModel.Id}", insuranseModel);
    }

    public async Task DeleteInsurance(int id)
    {
        var http = ApiAuthed();
        await http.DeleteAsync($"api/insurance/{id}");
    }

    public async Task<InsuranseModel?> GetRemainingMilage(int? vehicleId)
    {
        var http = ApiAuthed();
        using var resp = await http.GetAsync($"api/insurance/remaining-mileage/{vehicleId}", HttpCompletionOption.ResponseHeadersRead);

        // No insurance for this vehicle (404) or no content (204) => return null
        if (resp.StatusCode == HttpStatusCode.NotFound || resp.StatusCode == HttpStatusCode.NoContent)
            return null;

        resp.EnsureSuccessStatusCode();
        return await resp.Content.ReadFromJsonAsync<InsuranseModel>();
    }
}
