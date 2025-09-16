using ServiceApp.UI.Models;

namespace ServiceApp.UI.Services.ConsumptionRecordServices;

public interface IConsumptionRecordService
{

    Task<List<ConsumptionRecordModel>> GetAllConsumptionRecordsAsync(int vehicleId, DateTime? startDate = null, DateTime? endDate = null);
    Task<ConsumptionRecordModel?> GetConsumptionRecordByIdAsync(int id);
    Task CreateConsumptionRecordAsync(ConsumptionRecordModel consumptionRecord);
    Task UpdateConsumptionRecordAsync(ConsumptionRecordModel consumptionRecord);
    Task<bool> DeleteConsumptionRecordAsync(int id);
}
