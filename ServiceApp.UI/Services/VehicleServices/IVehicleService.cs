using ServiceApp.UI.Models;

namespace ServiceApp.UI.Services.VehicleServices;

public interface IVehicleService
{
    Task<List<VehicleModel>> GetAllVehiclesAsync();
    Task<VehicleModel?> GetVehicleByIdAsync(int id);
    Task<VehicleModel> AddVehicleAsync(VehicleModel vehicle);
    Task<VehicleModel> UpdateVehicleAsync(VehicleModel vehicle);
    Task<bool> DeleteVehicleAsync(int id);
}
