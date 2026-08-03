using System.ComponentModel.DataAnnotations;

namespace serviceApp.Server.Entities.FoodStorage;

public sealed class FoodStore
{
    public int Id { get; set; }
    public Guid FamilyId { get; set; }

    [MaxLength(150)]
    public string Name { get; set; } = string.Empty;

    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    public ICollection<FoodPurchase> Purchases { get; set; } = [];
}
