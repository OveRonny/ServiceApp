namespace serviceApp.Server.Features.FoodStorage.Contracts;

public sealed record FoodProductDto(
    int Id,
    string Barcode,
    string Name,
    string? Brand,
    string? QuantityLabel,
    string? ImageUrl,
    string Source);

public sealed record FoodStockItemDto(
    int Id,
    FoodProductDto Product,
    decimal Quantity,
    string Unit,
    string Location,
    DateOnly? BestBeforeDate,
    DateOnly? PurchasedDate);

public sealed record CreateFoodStockItemRequest(
    int FoodProductId,
    decimal Quantity,
    string Unit,
    string Location,
    DateOnly? BestBeforeDate,
    DateOnly? PurchasedDate);

public sealed record UpdateFoodStockItemRequest(
    decimal Quantity,
    string Unit,
    string Location,
    DateOnly? BestBeforeDate,
    DateOnly? PurchasedDate);

public sealed record CreateManualFoodProductRequest(
    string Name,
    string? Barcode,
    string? Brand,
    string? QuantityLabel);
