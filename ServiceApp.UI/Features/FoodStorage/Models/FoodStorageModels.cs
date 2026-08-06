using System.ComponentModel.DataAnnotations;

namespace ServiceApp.UI.Features.FoodStorage.Models;

public sealed record FoodProductModel(
    int Id, string Barcode, string Name, string? Brand,
    string? QuantityLabel, string? ImageUrl, string Source);

public sealed record FoodStockItemModel(
    int Id, FoodProductModel Product, decimal Quantity, string Unit,
    string Location, DateOnly? BestBeforeDate, DateOnly? PurchasedDate, int? CategoryId, string? Category,
    decimal? LatestUnitPrice, decimal EstimatedValue, decimal? MinimumQuantity);
public sealed record FoodStoreModel(int Id, string Name);
public sealed record FoodStorageLocationModel(int Id, string Name);
public sealed record FoodCategoryModel(int Id, string Name);

public static class FoodStockDateLabels
{
    public static bool IsFrozen(string? category) =>
        category?.Contains("frys", StringComparison.OrdinalIgnoreCase) == true;

    public static string ForCategory(string? category) =>
        IsFrozen(category) ? "Fryst dato" : "Best før";
}
public sealed record FoodPriceHistoryModel(int Id, int ProductId, string StoreName,
    decimal Quantity, string Unit, decimal TotalPrice, decimal UnitPrice, DateOnly PurchasedDate);
public sealed record FoodShoppingListItemModel(int StockItemId, string ProductName, string? Category,
    string Location, decimal CurrentQuantity, decimal MinimumQuantity, decimal RecommendedQuantity, string Unit);
public sealed record FoodStockWithdrawalModel(int Id, int ProductId, string ProductName,
    decimal Quantity, decimal RemainingQuantity, string Unit, DateTimeOffset RemovedAt);



public sealed class AddFoodStockModel
{
    [Required]
    public int FoodProductId { get; set; }
    [Required]
    public int? FoodCategoryId { get; set; }


    [Range(0.001, 999999)]
    public decimal Quantity { get; set; } = 1;
    [Range(0, 999999)]
    public decimal? MinimumQuantity { get; set; }


    [Required, StringLength(40)]
    public string Unit { get; set; } = "stk";

    [Required, StringLength(100)]
    public string Location { get; set; } = "Matlager";

    public DateOnly? BestBeforeDate { get; set; }
    public DateOnly? PurchasedDate { get; set; } = DateOnly.FromDateTime(DateTime.Today);

    public int? FoodStoreId { get; set; }

    [Range(0.01, 99999999)]
    public decimal? TotalPrice { get; set; }
}

public sealed class EditFoodStockModel
{
    [Range(0, 999999)]
    public decimal Quantity { get; set; }
    [Range(0, 999999)]
    public decimal? MinimumQuantity { get; set; }
    [Required]
    public int? FoodCategoryId { get; set; }
    [Required, StringLength(40)]
    public string Unit { get; set; } = "stk";
    [Required, StringLength(100)]
    public string Location { get; set; } = "Matlager";
    public DateOnly? BestBeforeDate { get; set; }
    public DateOnly? PurchasedDate { get; set; }
}

public sealed class ManualFoodProductModel
{
    [Required, StringLength(300)]
    public string Name { get; set; } = string.Empty;

    public string? Barcode { get; set; }
    public string? Brand { get; set; }
    public string? QuantityLabel { get; set; }
}
