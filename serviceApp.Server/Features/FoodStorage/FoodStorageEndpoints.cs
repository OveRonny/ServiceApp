using serviceApp.Server.Entities.FoodStorage;
using serviceApp.Server.Features.Autentication;
using serviceApp.Server.Features.FoodStorage.Contracts;
using serviceApp.Server.Features.FoodStorage.External;

namespace serviceApp.Server.Features.FoodStorage;

public sealed class FoodStorageEndpoints : IEndpointDefinition
{
    public void MapEndpoints(WebApplication app)
    {
        var group = app.MapGroup("/api/food-storage").RequireAuthorization().WithTags("Food storage");
        group.MapGet("/products/barcode/{barcode}", LookupBarcodeAsync);
        group.MapPost("/products/manual", CreateManualProductAsync);
        group.MapGet("/stock", GetStockAsync);
        group.MapPost("/stock", CreateStockItemAsync);
        group.MapPut("/stock/{id:int}", UpdateStockItemAsync);
        group.MapDelete("/stock/{id:int}", DeleteStockItemAsync);
        group.MapGet("/stores", GetStoresAsync);
        group.MapPost("/stores", CreateStoreAsync);
        group.MapGet("/products/{productId:int}/price-history", GetPriceHistoryAsync);
        group.MapGet("/locations", GetLocationsAsync);
        group.MapPost("/locations", CreateLocationAsync);
    }

    private static async Task<IResult> LookupBarcodeAsync(string barcode, ApplicationDbContext db,
        IOpenFoodFactsClient openFoodFacts, CancellationToken cancellationToken)
    {
        barcode = NormalizeBarcode(barcode);
        if (barcode.Length is < 8 or > 14)
            return Results.BadRequest("Strekkoden må inneholde mellom 8 og 14 sifre.");

        var cached = await db.FoodProducts.AsNoTracking()
            .SingleOrDefaultAsync(x => x.Barcode == barcode, cancellationToken);
        if (cached is not null)
            return Results.Ok(ToDto(cached));

        var external = await openFoodFacts.GetByBarcodeAsync(barcode, cancellationToken);
        if (external is null)
            return Results.NotFound();

        var product = new FoodProduct
        {
            Barcode = barcode, Name = external.Name, Brand = external.Brand,
            QuantityLabel = external.QuantityLabel, ImageUrl = external.ImageUrl,
            Source = "Open Food Facts", SourceUpdatedAt = DateTimeOffset.UtcNow
        };
        db.FoodProducts.Add(product);
        await db.SaveChangesAsync(cancellationToken);
        return Results.Ok(ToDto(product));
    }

    private static async Task<IResult> CreateManualProductAsync(CreateManualFoodProductRequest request,
        ApplicationDbContext db, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
            return Results.BadRequest("Produktnavn er påkrevd.");

        var barcode = NormalizeBarcode(request.Barcode ?? string.Empty);
        if (barcode.Length > 0 && barcode.Length is < 8 or > 14)
            return Results.BadRequest("Strekkoden må inneholde mellom 8 og 14 sifre.");

        if (barcode.Length > 0)
        {
            var existing = await db.FoodProducts.AsNoTracking()
                .SingleOrDefaultAsync(x => x.Barcode == barcode, cancellationToken);
            if (existing is not null)
                return Results.Ok(ToDto(existing));
        }

        var product = new FoodProduct
        {
            Barcode = barcode, Name = request.Name.Trim(), Brand = Clean(request.Brand),
            QuantityLabel = Clean(request.QuantityLabel), Source = "Manual"
        };
        db.FoodProducts.Add(product);
        await db.SaveChangesAsync(cancellationToken);
        return Results.Created($"/api/food-storage/products/{product.Id}", ToDto(product));
    }

    private static async Task<IResult> GetStockAsync(ApplicationDbContext db, CancellationToken cancellationToken)
    {
        var stock = await db.FoodStockItems.AsNoTracking().Include(x => x.FoodProduct)
            .OrderBy(x => x.BestBeforeDate == null).ThenBy(x => x.BestBeforeDate)
            .ThenBy(x => x.FoodProduct.Name).Select(x => ToDto(x)).ToListAsync(cancellationToken);
        return Results.Ok(stock);
    }

    private static async Task<IResult> CreateStockItemAsync(CreateFoodStockItemRequest request,
        ApplicationDbContext db, ICurrentUser currentUser, CancellationToken cancellationToken)
    {
        var familyId = await currentUser.GetFamilyIdAsync(cancellationToken);
        if (familyId is null) return Results.Unauthorized();
        var validation = ValidateStock(request.Quantity, request.Unit, request.Location);
        if (validation is not null) return Results.BadRequest(validation);
        if (request.FoodStoreId.HasValue != request.TotalPrice.HasValue)
            return Results.BadRequest("Butikk og pris må oppgis sammen.");
        if (request.TotalPrice is <= 0)
            return Results.BadRequest("Pris må være større enn null.");
        if (request.FoodStoreId is int storeId &&
            !await db.FoodStores.AnyAsync(x => x.Id == storeId, cancellationToken))
            return Results.BadRequest("Butikken finnes ikke.");

        if (!await db.FoodProducts.AnyAsync(x => x.Id == request.FoodProductId, cancellationToken))
            return Results.BadRequest("Produktet finnes ikke.");

        var item = new FoodStockItem
        {
            FamilyId = familyId.Value, FoodProductId = request.FoodProductId,
            Quantity = request.Quantity, Unit = request.Unit.Trim(), Location = request.Location.Trim(),
            BestBeforeDate = request.BestBeforeDate, PurchasedDate = request.PurchasedDate
        };
        db.FoodStockItems.Add(item);
        if (request.FoodStoreId is int purchaseStoreId && request.TotalPrice is decimal totalPrice)
        {
            db.FoodPurchases.Add(new FoodPurchase
            {
                FamilyId = familyId.Value, FoodProductId = request.FoodProductId,
                FoodStoreId = purchaseStoreId, Quantity = request.Quantity,
                Unit = request.Unit.Trim(), TotalPrice = totalPrice,
                PurchasedDate = request.PurchasedDate ?? DateOnly.FromDateTime(DateTime.UtcNow)
            });
        }

        await db.SaveChangesAsync(cancellationToken);
        await db.Entry(item).Reference(x => x.FoodProduct).LoadAsync(cancellationToken);
        return Results.Created($"/api/food-storage/stock/{item.Id}", ToDto(item));
    }

    private static async Task<IResult> UpdateStockItemAsync(int id, UpdateFoodStockItemRequest request,
        ApplicationDbContext db, CancellationToken cancellationToken)
    {
        var validation = ValidateStock(request.Quantity, request.Unit, request.Location);
        if (validation is not null) return Results.BadRequest(validation);
        var item = await db.FoodStockItems.Include(x => x.FoodProduct)
            .SingleOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (item is null) return Results.NotFound();

        item.Quantity = request.Quantity; item.Unit = request.Unit.Trim();
        item.Location = request.Location.Trim(); item.BestBeforeDate = request.BestBeforeDate;
        item.PurchasedDate = request.PurchasedDate; item.UpdatedAt = DateTimeOffset.UtcNow;
        await db.SaveChangesAsync(cancellationToken);
        return Results.Ok(ToDto(item));
    }

    private static async Task<IResult> DeleteStockItemAsync(int id, ApplicationDbContext db,
        CancellationToken cancellationToken)
    {
        var item = await db.FoodStockItems.SingleOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (item is null) return Results.NotFound();
        db.FoodStockItems.Remove(item);
        await db.SaveChangesAsync(cancellationToken);
        return Results.NoContent();
    }
    private static async Task<IResult> GetStoresAsync(ApplicationDbContext db, CancellationToken cancellationToken)
    {
        var stores = await db.FoodStores.AsNoTracking().OrderBy(x => x.Name)
            .Select(x => new FoodStoreDto(x.Id, x.Name)).ToListAsync(cancellationToken);
        return Results.Ok(stores);
    }

    private static async Task<IResult> CreateStoreAsync(CreateFoodStoreRequest request,
        ApplicationDbContext db, ICurrentUser currentUser, CancellationToken cancellationToken)
    {
        var familyId = await currentUser.GetFamilyIdAsync(cancellationToken);
        if (familyId is null) return Results.Unauthorized();
        var name = request.Name.Trim();
        if (name.Length is < 2 or > 150)
            return Results.BadRequest("Butikknavn må inneholde mellom 2 og 150 tegn.");
        var existing = await db.FoodStores.AsNoTracking()
            .SingleOrDefaultAsync(x => x.Name == name, cancellationToken);
        if (existing is not null) return Results.Ok(new FoodStoreDto(existing.Id, existing.Name));
        var store = new FoodStore { FamilyId = familyId.Value, Name = name };
        db.FoodStores.Add(store);
        await db.SaveChangesAsync(cancellationToken);
        return Results.Created($"/api/food-storage/stores/{store.Id}", new FoodStoreDto(store.Id, store.Name));
    }

    private static async Task<IResult> GetPriceHistoryAsync(int productId, ApplicationDbContext db,
        CancellationToken cancellationToken)
    {
        var history = await db.FoodPurchases.AsNoTracking()
            .Where(x => x.FoodProductId == productId)
            .OrderByDescending(x => x.PurchasedDate).ThenByDescending(x => x.Id)
            .Select(x => new FoodPriceHistoryDto(x.Id, x.FoodProductId, x.FoodStore.Name,
                x.Quantity, x.Unit, x.TotalPrice, x.Quantity == 0 ? 0 : x.TotalPrice / x.Quantity,
                x.PurchasedDate)).ToListAsync(cancellationToken);
        return Results.Ok(history);
    }


    private static async Task<IResult> GetLocationsAsync(ApplicationDbContext db, CancellationToken cancellationToken)
    {
        var locations = await db.FoodStorageLocations.AsNoTracking().OrderBy(x => x.Name)
            .Select(x => new FoodStorageLocationDto(x.Id, x.Name)).ToListAsync(cancellationToken);
        return Results.Ok(locations);
    }

    private static async Task<IResult> CreateLocationAsync(CreateFoodStorageLocationRequest request,
        ApplicationDbContext db, ICurrentUser currentUser, CancellationToken cancellationToken)
    {
        var familyId = await currentUser.GetFamilyIdAsync(cancellationToken);
        if (familyId is null) return Results.Unauthorized();
        var name = request.Name.Trim();
        if (name.Length is < 2 or > 100)
            return Results.BadRequest("Plassering må inneholde mellom 2 og 100 tegn.");
        var existing = await db.FoodStorageLocations.AsNoTracking()
            .SingleOrDefaultAsync(x => x.Name == name, cancellationToken);
        if (existing is not null) return Results.Ok(new FoodStorageLocationDto(existing.Id, existing.Name));
        var location = new FoodStorageLocation { FamilyId = familyId.Value, Name = name };
        db.FoodStorageLocations.Add(location);
        await db.SaveChangesAsync(cancellationToken);
        return Results.Created($"/api/food-storage/locations/{location.Id}", new FoodStorageLocationDto(location.Id, location.Name));
    }

    private static FoodProductDto ToDto(FoodProduct product) => new(product.Id, product.Barcode,
        product.Name, product.Brand, product.QuantityLabel, product.ImageUrl, product.Source);
    private static FoodStockItemDto ToDto(FoodStockItem item) => new(item.Id, ToDto(item.FoodProduct),
        item.Quantity, item.Unit, item.Location, item.BestBeforeDate, item.PurchasedDate);
    private static string NormalizeBarcode(string value) => new(value.Where(char.IsAsciiDigit).ToArray());
    private static string? Clean(string? value) => string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    private static string? ValidateStock(decimal quantity, string unit, string location) =>
        quantity <= 0 ? "Antall må være større enn null." :
        string.IsNullOrWhiteSpace(unit) ? "Enhet er påkrevd." :
        string.IsNullOrWhiteSpace(location) ? "Plassering er påkrevd." : null;
}
