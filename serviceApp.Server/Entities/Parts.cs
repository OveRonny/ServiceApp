namespace serviceApp.Server.Entities;

public class Parts
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal? Price { get; set; }
    public int Quantity { get; set; }
    public string Description { get; set; } = string.Empty;
    public int ServiceRecordId { get; set; }
    public ServiceRecord? ServiceRecord { get; set; }

    public int VehicleInventoryId { get; set; }
    public VehicleInventory? VehicleInventory { get; set; }
}
