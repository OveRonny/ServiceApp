using ServiceApp.UI.Models;
using System.Net.Http.Json;

namespace ServiceApp.UI.Services.ConsumptionRecordServices;

public class ConsumptionRecordService(HttpClient http) : IConsumptionRecordService
{
    private readonly HttpClient http = http;

    public async Task<List<ConsumptionRecordModel>> GetAllConsumptionRecordsAsync(int vehicleId, DateTime? startDate = null, DateTime? endDate = null)
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

        var result = await http.GetFromJsonAsync<List<ConsumptionRecordModel>>(query);
        return result ?? new List<ConsumptionRecordModel>();
    }

    public async Task<ConsumptionRecordModel> GetConsumptionRecordByIdAsync(int id)
    {
        var result = await http.GetFromJsonAsync<ConsumptionRecordModel>($"api/consumption-record/{id}");
        if (result == null)
        {
            return new ConsumptionRecordModel();
        }
        return result;
    }

    public async Task<ConsumptionRecordModel> CreateConsumptionRecordAsync(ConsumptionRecordModel consumptionRecord)
    {
        await http.PostAsJsonAsync("api/consumption-record", consumptionRecord);
        return consumptionRecord;
    }

    public async Task<ConsumptionRecordModel> UpdateConsumptionRecordAsync(ConsumptionRecordModel consumptionRecord)
    {
        await http.PutAsJsonAsync($"api/consumption-record/{consumptionRecord.Id}", consumptionRecord);
        return consumptionRecord;
    }

    public async Task<bool> DeleteConsumptionRecordAsync(int id)
    {
        await http.DeleteAsync($"api/consumption-record/{id}");
        return true;
    }
}
