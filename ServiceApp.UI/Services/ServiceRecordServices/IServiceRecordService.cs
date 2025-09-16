using ServiceApp.UI.Models;

namespace ServiceApp.UI.Services.ServiceRecordServices;

public interface IServiceRecordService
{
    Task<List<ServiceRecordModel>> GetServiceRecordsAsync(int vehicleId);
    Task<ServiceRecordModel> GetServiceRecordByIdAsync(int id);
    Task CreateServiceRecordAsync(ServiceRecordModel serviceRecord);
    Task UpdateServiceRecordAsync(ServiceRecordModel serviceRecord);
    Task<bool> DeleteServiceRecordAsync(int id);
}
