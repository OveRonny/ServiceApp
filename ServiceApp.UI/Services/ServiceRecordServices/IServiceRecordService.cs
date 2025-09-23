using Microsoft.AspNetCore.Components.Forms;
using ServiceApp.UI.Models;

namespace ServiceApp.UI.Services.ServiceRecordServices;

public interface IServiceRecordService
{
    Task<List<ServiceRecordModel>> GetServiceRecordsAsync(int vehicleId);
    Task<ServiceRecordModel> GetServiceRecordByIdAsync(int id);
    Task CreateServiceRecordAsync(ServiceRecordModel serviceRecord);
    Task UpdateServiceRecordAsync(ServiceRecordModel serviceRecord);
    Task<bool> DeleteServiceRecordAsync(int id);
    Task<bool> CreateServiceRecordWithImageAsync(ServiceRecordModel record, IBrowserFile? file, CancellationToken ct = default);
    Task<List<int>?> GetServiceRecordImageIdsAsync(int serviceRecordId, CancellationToken ct = default);
    Task<bool> UpdateServiceRecordWithImageAsync(ServiceRecordModel record, IBrowserFile? file);
    Task<string?> GetImageSasUrlAsync(int imageId);
    Task<Dictionary<int, string>> GetImageSasUrlsAsync(IEnumerable<int> imageIds);
}
