using ServiceApp.UI.Models;
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

    public async Task<InsuranseModel?> GetRemainingMilage(int vehicleId)
    {
        var http = ApiAuthed();
        var result = await http.GetFromJsonAsync<InsuranseModel>($"api/insurance/remaining-mileage/{vehicleId}");
        return result;
    }
}
