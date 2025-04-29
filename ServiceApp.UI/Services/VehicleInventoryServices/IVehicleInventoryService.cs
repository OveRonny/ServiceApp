using ServiceApp.UI.Models;

namespace ServiceApp.UI.Services.VehicleInventoryServices;

public interface IVehicleInventoryService
{
    Task<List<VehicleInventoryModel>> GetVehicleInventoryAsync(int vehicleId);
    Task<VehicleInventoryModel> CreateVehicleInventoryAsync(VehicleInventoryModel vehicleInventory);
    Task<VehicleInventoryModel> UpdateVehicleInventoryAsync(VehicleInventoryModel vehicleInventory);
    Task<bool> DeleteVehicleInventoryAsync(int id);
}
