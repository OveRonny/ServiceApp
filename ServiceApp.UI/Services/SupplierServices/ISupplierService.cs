using ServiceApp.UI.Models;

namespace ServiceApp.UI.Services.SupplierServices;

public interface ISupplierService
{
    Task<List<SupplierModel>> GetSuppliersAsync();
    Task<SupplierModel> GetSupplierByIdAsync(int id);
    Task<SupplierModel> CreateSupplierAsync(SupplierModel supplier);
    Task<SupplierModel> UpdateSupplierAsync(SupplierModel supplier);
    Task<bool> DeleteSupplierAsync(int id);
}
