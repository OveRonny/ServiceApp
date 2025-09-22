using ServiceApp.UI.Models;
using System.Net.Http.Json;

namespace ServiceApp.UI.Services.ConsumptionRecordServices;

public class ConsumptionRecordService(IHttpClientFactory clients) : IConsumptionRecordService
{
    private readonly IHttpClientFactory _clients = clients;

    private HttpClient ApiAuthed() => _clients.CreateClient("ApiAuthed");

    public async Task<ConsumptionRecordsWithSummaryModel> GetAllConsumptionRecordsAsync(int vehicleId, DateTime? startDate = null, DateTime? endDate = null)
    {
        var query = $"api/consumption-record/vehicle/{vehicleId}?";

        if (startDate.HasValue)
        {
            query += $"startDate={startDate.Value:yyyy-MM-dd}&";
        }

        if (endDate.HasValue)
        {
            query += $"endDate={endDate.Value:yyyy-MM-dd}&";
        }

        var http = ApiAuthed();

        return await http.GetFromJsonAsync<ConsumptionRecordsWithSummaryModel>(query)
           ?? new ConsumptionRecordsWithSummaryModel();
    }

    public async Task<ConsumptionRecordModel?> GetConsumptionRecordByIdAsync(int id)
    {
        var http = ApiAuthed();
        return await http.GetFromJsonAsync<ConsumptionRecordModel>($"api/consumption-record/{id}");

    }

    public async Task CreateConsumptionRecordAsync(ConsumptionRecordModel consumptionRecord)
    {
        var http = ApiAuthed();
        await http.PostAsJsonAsync("api/consumption-record", consumptionRecord);
    }

    public async Task UpdateConsumptionRecordAsync(ConsumptionRecordModel consumptionRecord)
    {
        var http = ApiAuthed();
        await http.PutAsJsonAsync($"api/consumption-record/{consumptionRecord.Id}", consumptionRecord);
    }

    public async Task<bool> DeleteConsumptionRecordAsync(int id)
    {
        var http = ApiAuthed();
        var resp = await http.DeleteAsync($"api/consumption-record/{id}");
        return resp.IsSuccessStatusCode;
    }
}
