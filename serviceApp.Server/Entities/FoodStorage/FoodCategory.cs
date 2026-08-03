using System.ComponentModel.DataAnnotations;

namespace serviceApp.Server.Entities.FoodStorage;

public sealed class FoodCategory
{
    public int Id { get; set; }
    public Guid FamilyId { get; set; }

    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    public ICollection<FoodStockItem> StockItems { get; set; } = [];
}
