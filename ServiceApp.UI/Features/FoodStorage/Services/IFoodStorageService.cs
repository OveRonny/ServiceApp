using ServiceApp.UI.Features.FoodStorage.Models;

namespace ServiceApp.UI.Features.FoodStorage.Services;

public interface IFoodStorageService
{
    Task<IReadOnlyList<FoodStockItemModel>> GetStockAsync(CancellationToken cancellationToken = default);
    Task<FoodProductModel?> LookupBarcodeAsync(string barcode, CancellationToken cancellationToken = default);
    Task<FoodProductModel> CreateManualProductAsync(ManualFoodProductModel model, CancellationToken cancellationToken = default);
    Task AddStockAsync(AddFoodStockModel model, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<FoodStoreModel>> GetStoresAsync(CancellationToken cancellationToken = default);
    Task<FoodStoreModel> CreateStoreAsync(string name, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<FoodPriceHistoryModel>> GetPriceHistoryAsync(int productId, CancellationToken cancellationToken = default);
    Task DeleteStockAsync(int id, CancellationToken cancellationToken = default);
}
