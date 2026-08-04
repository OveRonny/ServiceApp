using ServiceApp.UI.Features.FoodStorage.Models;

namespace ServiceApp.UI.Features.FoodStorage.Services;

public interface IFoodStorageService
{
    Task<IReadOnlyList<FoodStockItemModel>> GetStockAsync(CancellationToken cancellationToken = default);
    Task<FoodProductModel?> LookupBarcodeAsync(string barcode, CancellationToken cancellationToken = default);
    Task<FoodProductModel> CreateManualProductAsync(ManualFoodProductModel model, CancellationToken cancellationToken = default);
    Task AddStockAsync(AddFoodStockModel model, CancellationToken cancellationToken = default);
    Task RecordStockPriceAsync(int stockItemId, int foodStoreId, decimal totalPrice,
        DateOnly? purchasedDate, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<FoodStoreModel>> GetStoresAsync(CancellationToken cancellationToken = default);
    Task UpdateStockAsync(int id, EditFoodStockModel model, CancellationToken cancellationToken = default);
    Task<FoodStoreModel> CreateStoreAsync(string name, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<FoodPriceHistoryModel>> GetPriceHistoryAsync(int productId, CancellationToken cancellationToken = default);
    Task WithdrawStockAsync(int productId, decimal quantity, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<FoodStockWithdrawalModel>> GetStockWithdrawalsAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<FoodStorageLocationModel>> GetLocationsAsync(CancellationToken cancellationToken = default);
    Task<FoodStorageLocationModel> CreateLocationAsync(string name, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<FoodCategoryModel>> GetCategoriesAsync(CancellationToken cancellationToken = default);
    Task<FoodCategoryModel> CreateCategoryAsync(string name, CancellationToken cancellationToken = default);
    Task DeleteStockAsync(int id, CancellationToken cancellationToken = default);
    Task SetQuantityAsync(int id, decimal quantity, CancellationToken cancellationToken = default);
    Task SetMinimumQuantityAsync(int id, decimal? minimumQuantity, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<FoodShoppingListItemModel>> GetShoppingListAsync(CancellationToken cancellationToken = default);
}
