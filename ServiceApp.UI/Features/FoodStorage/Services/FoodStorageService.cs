using System.Net;
using System.Net.Http.Json;
using ServiceApp.UI.Features.FoodStorage.Models;

namespace ServiceApp.UI.Features.FoodStorage.Services;

public sealed class FoodStorageService(IHttpClientFactory clients) : IFoodStorageService
{
    private HttpClient Api() => clients.CreateClient("ApiAuthed");

    public async Task<IReadOnlyList<FoodStockItemModel>> GetStockAsync(CancellationToken cancellationToken = default) =>
        await Api().GetFromJsonAsync<List<FoodStockItemModel>>("api/food-storage/stock", cancellationToken) ?? [];

    public async Task<FoodProductModel?> LookupBarcodeAsync(string barcode, CancellationToken cancellationToken = default)
    {
        using var response = await Api().GetAsync(
            $"api/food-storage/products/barcode/{Uri.EscapeDataString(barcode)}", cancellationToken);
        if (response.StatusCode == HttpStatusCode.NotFound) return null;
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<FoodProductModel>(cancellationToken);
    }

    public async Task<FoodProductModel> CreateManualProductAsync(ManualFoodProductModel model,
        CancellationToken cancellationToken = default)
    {
        using var response = await Api().PostAsJsonAsync("api/food-storage/products/manual", model, cancellationToken);
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<FoodProductModel>(cancellationToken))!;
    }

    public async Task<IReadOnlyList<FoodStoreModel>> GetStoresAsync(CancellationToken cancellationToken = default) =>
        await Api().GetFromJsonAsync<List<FoodStoreModel>>("api/food-storage/stores", cancellationToken) ?? [];

    public async Task<FoodStoreModel> CreateStoreAsync(string name, CancellationToken cancellationToken = default)
    {
        using var response = await Api().PostAsJsonAsync("api/food-storage/stores", new { name }, cancellationToken);
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<FoodStoreModel>(cancellationToken))!;
    }

    public async Task<IReadOnlyList<FoodPriceHistoryModel>> GetPriceHistoryAsync(int productId,
        CancellationToken cancellationToken = default) =>
        await Api().GetFromJsonAsync<List<FoodPriceHistoryModel>>(
            $"api/food-storage/products/{productId}/price-history", cancellationToken) ?? [];

    public async Task<IReadOnlyList<FoodStorageLocationModel>> GetLocationsAsync(CancellationToken cancellationToken = default) =>
        await Api().GetFromJsonAsync<List<FoodStorageLocationModel>>("api/food-storage/locations", cancellationToken) ?? [];

    public async Task<FoodStorageLocationModel> CreateLocationAsync(string name, CancellationToken cancellationToken = default)
    {
        using var response = await Api().PostAsJsonAsync("api/food-storage/locations", new { name }, cancellationToken);
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<FoodStorageLocationModel>(cancellationToken))!;
    }

    public async Task<IReadOnlyList<FoodCategoryModel>> GetCategoriesAsync(CancellationToken cancellationToken = default) =>
        await Api().GetFromJsonAsync<List<FoodCategoryModel>>("api/food-storage/categories", cancellationToken) ?? [];

    public async Task<FoodCategoryModel> CreateCategoryAsync(string name, CancellationToken cancellationToken = default)
    {
        using var response = await Api().PostAsJsonAsync("api/food-storage/categories", new { name }, cancellationToken);
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<FoodCategoryModel>(cancellationToken))!;
    }

    public async Task AddStockAsync(AddFoodStockModel model, CancellationToken cancellationToken = default)
    {
        using var response = await Api().PostAsJsonAsync("api/food-storage/stock", model, cancellationToken);
        response.EnsureSuccessStatusCode();
    }

    public async Task UpdateStockAsync(int id, EditFoodStockModel model, CancellationToken cancellationToken = default)
    {
        using var response = await Api().PutAsJsonAsync($"api/food-storage/stock/{id}", model, cancellationToken);
        response.EnsureSuccessStatusCode();
    }

    public async Task DeleteStockAsync(int id, CancellationToken cancellationToken = default)
    {
        using var response = await Api().DeleteAsync($"api/food-storage/stock/{id}", cancellationToken);
        response.EnsureSuccessStatusCode();
    }

    public async Task SetMinimumQuantityAsync(int id, decimal? minimumQuantity,
        CancellationToken cancellationToken = default)
    {
        using var response = await Api().PutAsJsonAsync($"api/food-storage/stock/{id}/minimum",
            new { minimumQuantity }, cancellationToken);
        response.EnsureSuccessStatusCode();
    }

    public async Task WithdrawStockAsync(int productId, decimal quantity,
        CancellationToken cancellationToken = default)
    {
        using var response = await Api().PostAsJsonAsync("api/food-storage/stock/withdraw",
            new { productId, quantity }, cancellationToken);
        response.EnsureSuccessStatusCode();
    }

    public async Task<IReadOnlyList<FoodStockWithdrawalModel>> GetStockWithdrawalsAsync(
        CancellationToken cancellationToken = default) =>
        await Api().GetFromJsonAsync<List<FoodStockWithdrawalModel>>(
            "api/food-storage/stock/withdrawals", cancellationToken) ?? [];

    public async Task<IReadOnlyList<FoodShoppingListItemModel>> GetShoppingListAsync(
        CancellationToken cancellationToken = default) =>
        await Api().GetFromJsonAsync<List<FoodShoppingListItemModel>>(
            "api/food-storage/shopping-list", cancellationToken) ?? [];

    public async Task SetQuantityAsync(int id, decimal quantity,
        CancellationToken cancellationToken = default)
    {
        using var response = await Api().PutAsJsonAsync($"api/food-storage/stock/{id}/quantity",
            new { quantity }, cancellationToken);
        response.EnsureSuccessStatusCode();
    }

}
