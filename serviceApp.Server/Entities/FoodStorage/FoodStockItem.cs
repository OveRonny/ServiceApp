using System.ComponentModel.DataAnnotations;

namespace serviceApp.Server.Entities.FoodStorage;

public sealed class FoodStockItem
{
    public int Id { get; set; }
    public Guid FamilyId { get; set; }
    public int FoodProductId { get; set; }
    public FoodProduct FoodProduct { get; set; } = null!;
    public int? FoodCategoryId { get; set; }
    public FoodCategory? FoodCategory { get; set; }

    [Precision(18, 3)]
    public decimal Quantity { get; set; }
    [Precision(18, 3)]
    public decimal? MinimumQuantity { get; set; }


    [MaxLength(40)]
    public string Unit { get; set; } = "stk";

    [MaxLength(100)]
    public string Location { get; set; } = "Matlager";

    public DateOnly? BestBeforeDate { get; set; }
    public DateOnly? PurchasedDate { get; set; }
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset UpdatedAt { get; set; } = DateTimeOffset.UtcNow;

    [Timestamp]
    public byte[] RowVersion { get; set; } = [];
}
