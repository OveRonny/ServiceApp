namespace ServiceApp.UI.Models;

public class UsedPartModel
{
    // Stable key for UI rows to avoid index-based binding issues
    public Guid RowId { get; set; } = Guid.NewGuid();

    public int? VehicleInventoryId { get; set; }
    public int Quantity { get; set; }

    // UI-only: unit cost and description for display
    public decimal? UnitCost { get; set; }
    public string? Description { get; set; }
    public string? Name { get; set; }
}
