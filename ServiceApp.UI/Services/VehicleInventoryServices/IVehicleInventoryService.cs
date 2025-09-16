using ServiceApp.UI.Models;

namespace ServiceApp.UI.Services.VehicleInventoryServices;

public interface IVehicleInventoryService
{
    Task<List<VehicleInventoryModel>> GetVehicleInventoryAsync(int vehicleId);
    Task<VehicleInventoryModel?> GetVehicleInventoryByIdAsync(int id);
    Task CreateVehicleInventoryAsync(VehicleInventoryModel vehicleInventory);
    Task UpdateVehicleInventoryAsync(VehicleInventoryModel vehicleInventory);
    Task DeleteVehicleInventoryAsync(int id);
}
