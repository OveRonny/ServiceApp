namespace ServiceApp.UI.Models;

public class VehicleInventoryModel
{
    public int Id { get; set; }
    public string PartName { get; set; } = string.Empty;
    public int? QuantityInStock { get; set; }
    public int? ReorderThreshold { get; set; }

    public string Description { get; set; } = string.Empty;
    public decimal Cost { get; set; }
    public DateTime PurchaseDate { get; set; }

    public int VehicleId { get; set; }
    public VehicleModel? Vehicle { get; set; }
    public int SupplierId { get; set; }
    public SupplierModel? Supplier { get; set; }

    public ICollection<PartsModel> Parts { get; set; } = new List<PartsModel>();
}
