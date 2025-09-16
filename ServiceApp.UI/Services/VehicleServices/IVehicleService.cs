using ServiceApp.UI.Models;

namespace ServiceApp.UI.Services.VehicleServices;

public interface IVehicleService
{
    Task<List<VehicleModel>> GetAllVehiclesAsync();
    Task<VehicleModel?> GetVehicleByIdAsync(int id);
    Task<List<VehicleModel>> GetVehiclesByOwnerIdAsync(int ownerId);
    Task AddVehicleAsync(VehicleModel model);
    Task UpdateVehicleAsync(VehicleModel model);
    Task DeleteVehicleAsync(int id);
}
