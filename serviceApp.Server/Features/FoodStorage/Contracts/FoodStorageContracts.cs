namespace serviceApp.Server.Features.FoodStorage.Contracts;
public sealed record RecordFoodStockPriceRequest(int FoodStoreId, decimal TotalPrice, DateOnly? PurchasedDate);

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
    DateOnly? PurchasedDate,
    int? CategoryId,
    string? Category,
    decimal? LatestUnitPrice,
    decimal EstimatedValue,
    decimal? MinimumQuantity);
public sealed record FoodStoreDto(int Id, string Name);
public sealed record FoodStorageLocationDto(int Id, string Name);
public sealed record FoodCategoryDto(int Id, string Name);

public sealed record FoodPriceHistoryDto(
    int Id, int ProductId, string StoreName, decimal Quantity, string Unit,
    decimal TotalPrice, decimal UnitPrice, DateOnly PurchasedDate);

public sealed record FoodShoppingListItemDto(int StockItemId, string ProductName, string? Category,
    string Location, decimal CurrentQuantity, decimal MinimumQuantity,
    decimal RecommendedQuantity, string Unit);

public sealed record FoodStockWithdrawalDto(int Id, int ProductId, string ProductName,
    decimal Quantity, decimal RemainingQuantity, string Unit, DateTimeOffset RemovedAt);
public sealed record WithdrawFoodStockRequest(int ProductId, decimal Quantity);
public sealed record CreateFoodStoreRequest(string Name);

public sealed record CreateFoodStorageLocationRequest(string Name);
public sealed record CreateFoodCategoryRequest(string Name);

public sealed record CreateFoodStockItemRequest(
    int FoodProductId,
    int? FoodCategoryId,
    decimal Quantity,
    decimal? MinimumQuantity,
    string Unit,
    string Location,
    DateOnly? BestBeforeDate,
    DateOnly? PurchasedDate,
    int? FoodStoreId,
    decimal? TotalPrice);

public sealed record UpdateFoodStockItemRequest(
    decimal Quantity,
    int? FoodCategoryId,
    decimal? MinimumQuantity,
    string Unit,
    string Location,
    DateOnly? BestBeforeDate,
    DateOnly? PurchasedDate);
public sealed record SetStockQuantityRequest(decimal Quantity);


public sealed record SetMinimumQuantityRequest(decimal? MinimumQuantity);
public sealed record CreateManualFoodProductRequest(
    string Name,
    string? Barcode,
    string? Brand,
    string? QuantityLabel);
