namespace serviceApp.Server.Features.FoodStorage.External;

public interface IOpenFoodFactsClient
{
    Task<ExternalFoodProduct?> GetByBarcodeAsync(string barcode, CancellationToken cancellationToken);
}

public sealed record ExternalFoodProduct(
    string Barcode,
    string Name,
    string? Brand,
    string? QuantityLabel,
    string? ImageUrl);
