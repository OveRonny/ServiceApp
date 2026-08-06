namespace serviceApp.Server.Entities.FoodStorage;

public sealed class FoodStockBatch
{
    public int Id { get; set; }
    public Guid FamilyId { get; set; }
    public int FoodStockItemId { get; set; }
    public FoodStockItem FoodStockItem { get; set; } = null!;
    [Precision(18, 3)] public decimal Quantity { get; set; }
    public DateOnly? BestBeforeDate { get; set; }
    public DateOnly? FrozenDate { get; set; }
    public DateOnly? PurchasedDate { get; set; }
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
}
