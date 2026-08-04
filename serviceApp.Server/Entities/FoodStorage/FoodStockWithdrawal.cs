using Microsoft.EntityFrameworkCore;

namespace serviceApp.Server.Entities.FoodStorage;

public sealed class FoodStockWithdrawal
{
    public int Id { get; set; }
    public Guid FamilyId { get; set; }
    public int FoodProductId { get; set; }
    public FoodProduct FoodProduct { get; set; } = null!;
    [Precision(18, 3)] public decimal Quantity { get; set; }
    [Precision(18, 3)] public decimal RemainingQuantity { get; set; }
    public string Unit { get; set; } = string.Empty;
    public DateTimeOffset RemovedAt { get; set; } = DateTimeOffset.UtcNow;
}
