using ServiceApp.UI.Models;

namespace ServiceApp.UI.Services.ServiceTypeServices;

public interface IServiceTypeService
{
    Task<List<ServiceTypeModel>> GetServiceTypesAsync();
    Task<ServiceTypeModel?> GetServiceTypeByIdAsync(int id);
    Task CreateServiceTypeAsync(ServiceTypeModel serviceType);
    Task UpdateServiceTypeAsync(ServiceTypeModel serviceType);
    Task<bool> DeleteServiceTypeAsync(int id);
}
