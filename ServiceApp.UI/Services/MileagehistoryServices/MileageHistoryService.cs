using ServiceApp.UI.Models;
using System.Net.Http.Json;

namespace ServiceApp.UI.Services.MileagehistoryServices;

public class MileageHistoryService(IHttpClientFactory clients) : IMileageHistoryService
{
    private readonly IHttpClientFactory _clients = clients;

    private HttpClient ApiAuthed() => _clients.CreateClient("ApiAuthed");
    public async Task<List<MileageHistoryModel>> GetAllMileageHistories(int vehicleId)
    {
        var http = ApiAuthed();
        return await http.GetFromJsonAsync<List<MileageHistoryModel>>($"api/mileagehistory/vehicle/{vehicleId}") ?? [];
    }

    public async Task<MileageHistoryModel?> GetMileageHistoryById(int id)
    {
        var http = ApiAuthed();
        return await http.GetFromJsonAsync<MileageHistoryModel>($"api/mileagehistory/{id}");
    }

    public async Task CreateMileageHistory(MileageHistoryModel mileageHistoryModel)
    {
        var http = ApiAuthed();
        await http.PostAsJsonAsync("api/mileagehistory", mileageHistoryModel);
    }

    public async Task UpdateMileageHistory(MileageHistoryModel mileageHistoryModel)
    {
        var http = ApiAuthed();
        await http.PutAsJsonAsync($"api/mileagehistory/{mileageHistoryModel.Id}", mileageHistoryModel);
    }

    public async Task DeleteMileageHistory(int id)
    {
        var http = ApiAuthed();
        await http.DeleteAsync($"api/mileagehistory/{id}");
    }
}
