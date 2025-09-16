using ServiceApp.UI.Models;

namespace ServiceApp.UI.Services.SupplierServices;

public interface ISupplierService
{
    Task<List<SupplierModel>> GetSuppliersAsync();
    Task<SupplierModel?> GetSupplierByIdAsync(int id);
    Task CreateSupplierAsync(SupplierModel supplier);
    Task UpdateSupplierAsync(SupplierModel supplier);
    Task<bool> DeleteSupplierAsync(int id);
}
