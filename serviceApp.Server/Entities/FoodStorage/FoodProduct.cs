using System.ComponentModel.DataAnnotations;

namespace serviceApp.Server.Entities.FoodStorage;

public sealed class FoodProduct
{
    public int Id { get; set; }

    [MaxLength(32)]
    public string Barcode { get; set; } = string.Empty;

    [MaxLength(300)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(200)]
    public string? Brand { get; set; }

    [MaxLength(100)]
    public string? QuantityLabel { get; set; }

    [MaxLength(1000)]
    public string? ImageUrl { get; set; }

    [MaxLength(100)]
    public string Source { get; set; } = "Manual";

    public DateTimeOffset? SourceUpdatedAt { get; set; }
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

    public ICollection<FoodStockItem> StockItems { get; set; } = [];
}
