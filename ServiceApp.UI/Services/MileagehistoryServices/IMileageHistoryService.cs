using ServiceApp.UI.Models;

namespace ServiceApp.UI.Services.MileagehistoryServices;

public interface IMileageHistoryService
{
    Task<List<MileageHistoryModel>> GetAllMileageHistories(int vehicleId);
    Task<MileageHistoryModel?> GetMileageHistoryById(int id);
    Task CreateMileageHistory(MileageHistoryModel mileageHistoryModel);
    Task UpdateMileageHistory(MileageHistoryModel mileageHistoryModel);
    Task DeleteMileageHistory(int id);
}
