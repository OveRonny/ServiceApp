using ServiceApp.UI.Models;

namespace ServiceApp.UI.Services.ServiceCompanyServices;

public interface IServiceCompanyService
{
    Task<List<ServiceCompanyModel>> GetAllServiceCompaniesAsync();
    Task<ServiceCompanyModel?> GetServiceCompanyByIdAsync(int id);
    Task<ServiceCompanyModel> AddServiceCompanyAsync(ServiceCompanyModel serviceCompany);
    Task<ServiceCompanyModel> UpdateServiceCompanyAsync(ServiceCompanyModel serviceCompany);
    Task<bool> DeleteServiceCompanyAsync(int id);
}
