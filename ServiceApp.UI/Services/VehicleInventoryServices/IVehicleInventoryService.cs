using ServiceApp.UI.Models;

namespace ServiceApp.UI.Services.VehicleInventoryServices;

public interface IVehicleInventoryService
{
    Task<List<VehicleInventoryModel>> GetVehicleInventoryAsync(int vehicleId);
    Task<List<VehicleInventoryModel>> GetVehicleInventoryAsync(int vehicleId, bool includeZero);
    Task<VehicleInventoryModel?> GetVehicleInventoryByIdAsync(int id);
    Task CreateVehicleInventoryAsync(VehicleInventoryModel vehicleInventory);
    Task UpdateVehicleInventoryAsync(VehicleInventoryModel vehicleInventory);
    Task DeleteVehicleInventoryAsync(int id);
    Task AdjustInventoryAsync(
        int id,
        decimal quantityDelta,
        string partName,
        string description,
        decimal cost,
        int vehicleId,
        int supplierId,
        decimal? reorderThreshold,
        UnitOfMeasure unit);
}
