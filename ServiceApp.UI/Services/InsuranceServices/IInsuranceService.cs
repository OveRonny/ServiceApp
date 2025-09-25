using ServiceApp.UI.Models;

namespace ServiceApp.UI.Services.InsuranceServices;

public interface IInsuranceService
{
    Task<List<InsuranseModel>> GetAllInsurances();
    Task<InsuranseModel?> GetRemainingMilage(int vehicleId);
    Task<InsuranseModel?> GetInsuranceById(int id);
    Task CreateInsurance(InsuranseModel insuranseModel);
    Task UpdateInsurance(InsuranseModel insuranseModel);
    Task DeleteInsurance(int id);
}
