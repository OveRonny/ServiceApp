namespace serviceApp.Server.Entities;

public class VehicleInventory
{
    public int Id { get; set; }
    public string PartName { get; set; } = string.Empty;
    public int? QuantityInStock { get; set; }
    public int? ReorderThreshold { get; set; }

    public string Description { get; set; } = string.Empty;
    public decimal Cost { get; set; }
    public DateTime PurchaseDate { get; set; }
    public int VehicleId { get; set; }
    public Vehicle? Vehicle { get; set; }
    public int SupplierId { get; set; }
    public Supplier? Supplier { get; set; }
    public Guid FamilyId { get; set; }

    public ICollection<Parts> Parts { get; set; } = new List<Parts>();

    public bool NeedsReorder()
    {
        if (QuantityInStock.HasValue && ReorderThreshold.HasValue)
        {
            return QuantityInStock.Value <= ReorderThreshold.Value;
        }
        return false;
    }
}
