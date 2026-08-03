using System.ComponentModel.DataAnnotations;

namespace serviceApp.Server.Entities.FoodStorage;

public sealed class FoodPurchase
{
    public int Id { get; set; }
    public Guid FamilyId { get; set; }
    public int FoodProductId { get; set; }
    public FoodProduct FoodProduct { get; set; } = null!;
    public int FoodStoreId { get; set; }
    public FoodStore FoodStore { get; set; } = null!;

    [Precision(18, 3)]
    public decimal Quantity { get; set; }

    [MaxLength(40)]
    public string Unit { get; set; } = "stk";

    [Precision(18, 2)]
    public decimal TotalPrice { get; set; }

    public DateOnly PurchasedDate { get; set; }
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
}
