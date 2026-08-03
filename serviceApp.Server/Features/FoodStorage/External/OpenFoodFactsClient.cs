using System.Net;
using System.Net.Http.Json;
using System.Text.Json.Serialization;

namespace serviceApp.Server.Features.FoodStorage.External;

public sealed class OpenFoodFactsClient(HttpClient httpClient, ILogger<OpenFoodFactsClient> logger)
    : IOpenFoodFactsClient
{
    public async Task<ExternalFoodProduct?> GetByBarcodeAsync(
        string barcode,
        CancellationToken cancellationToken)
    {
        using var response = await httpClient.GetAsync(
            $"api/v3/product/{Uri.EscapeDataString(barcode)}?fields=code,product_name,brands,quantity,image_front_small_url",
            cancellationToken);

        if (response.StatusCode == HttpStatusCode.NotFound)
            return null;

        if (!response.IsSuccessStatusCode)
        {
            logger.LogWarning(
                "Open Food Facts returned {StatusCode} for barcode {Barcode}",
                response.StatusCode,
                barcode);
            return null;
        }

        var payload = await response.Content.ReadFromJsonAsync<ProductResponse>(cancellationToken);
        var product = payload?.Product;

        if (payload?.Status != "success" || product is null || string.IsNullOrWhiteSpace(product.Name))
            return null;

        return new ExternalFoodProduct(
            product.Code ?? barcode,
            product.Name.Trim(),
            product.Brands,
            product.Quantity,
            product.ImageUrl);
    }

    private sealed class ProductResponse
    {
        [JsonPropertyName("status")]
        public string? Status { get; init; }

        [JsonPropertyName("product")]
        public ProductPayload? Product { get; init; }
    }

    private sealed class ProductPayload
    {
        [JsonPropertyName("code")]
        public string? Code { get; init; }

        [JsonPropertyName("product_name")]
        public string? Name { get; init; }

        [JsonPropertyName("brands")]
        public string? Brands { get; init; }

        [JsonPropertyName("quantity")]
        public string? Quantity { get; init; }

        [JsonPropertyName("image_front_small_url")]
        public string? ImageUrl { get; init; }
    }
}
