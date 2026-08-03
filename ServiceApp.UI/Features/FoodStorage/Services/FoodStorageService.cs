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

    public async Task AddStockAsync(AddFoodStockModel model, CancellationToken cancellationToken = default)
    {
        using var response = await Api().PostAsJsonAsync("api/food-storage/stock", model, cancellationToken);
        response.EnsureSuccessStatusCode();
    }

    public async Task DeleteStockAsync(int id, CancellationToken cancellationToken = default)
    {
        using var response = await Api().DeleteAsync($"api/food-storage/stock/{id}", cancellationToken);
        response.EnsureSuccessStatusCode();
    }
}
