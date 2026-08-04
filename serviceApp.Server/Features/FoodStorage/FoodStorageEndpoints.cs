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
        group.MapPut("/stock/{id:int}/minimum", SetMinimumQuantityAsync);
        group.MapGet("/shopping-list", GetShoppingListAsync);
        group.MapPut("/stock/{id:int}/quantity", SetQuantityAsync);
        group.MapPost("/stock/withdraw", WithdrawStockAsync);
        group.MapGet("/stock/withdrawals", GetStockWithdrawalsAsync);
        group.MapGet("/stores", GetStoresAsync);
        group.MapPost("/stores", CreateStoreAsync);
        group.MapGet("/products/{productId:int}/price-history", GetPriceHistoryAsync);
        group.MapGet("/locations", GetLocationsAsync);
        group.MapPost("/locations", CreateLocationAsync);
        group.MapGet("/categories", GetCategoriesAsync);
        group.MapPost("/categories", CreateCategoryAsync);
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
        var items = await db.FoodStockItems.AsNoTracking().Include(x => x.FoodProduct)
            .Include(x => x.FoodCategory)
            .OrderBy(x => x.BestBeforeDate == null).ThenBy(x => x.BestBeforeDate)
            .ThenBy(x => x.FoodProduct.Name).ToListAsync(cancellationToken);

        var productIds = items.Select(x => x.FoodProductId).Distinct().ToArray();
        var purchases = await db.FoodPurchases.AsNoTracking()
            .Where(x => productIds.Contains(x.FoodProductId))
            .OrderByDescending(x => x.PurchasedDate).ThenByDescending(x => x.Id)
            .Select(x => new { x.FoodProductId, x.TotalPrice, x.Quantity })
            .ToListAsync(cancellationToken);

        var averagePrices = purchases.GroupBy(x => x.FoodProductId)
            .ToDictionary(x => x.Key, x => x.Sum(p => p.Quantity) == 0
                ? (decimal?)null
                : x.Sum(p => p.TotalPrice) / x.Sum(p => p.Quantity));

        var stock = items.Select(item =>
        {
            averagePrices.TryGetValue(item.FoodProductId, out var unitPrice);
            return ToDto(item, unitPrice);
        }).ToList();
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
        if (request.FoodCategoryId is int categoryId &&
            !await db.FoodCategories.AnyAsync(x => x.Id == categoryId, cancellationToken))
            return Results.BadRequest("Kategorien finnes ikke.");

        var item = await db.FoodStockItems
            .Include(x => x.FoodProduct)
            .Include(x => x.FoodCategory)
            .SingleOrDefaultAsync(x => x.FoodProductId == request.FoodProductId, cancellationToken);
        var isNew = item is null;
        if (item is null)
        {
            item = new FoodStockItem
            {
                FamilyId = familyId.Value, FoodProductId = request.FoodProductId,
                Quantity = request.Quantity, Unit = request.Unit.Trim(), Location = request.Location.Trim(),
                FoodCategoryId = request.FoodCategoryId,
                BestBeforeDate = request.BestBeforeDate, PurchasedDate = request.PurchasedDate,
                MinimumQuantity = request.MinimumQuantity,
            };
            db.FoodStockItems.Add(item);
        }
        else
        {
            item.Quantity += request.Quantity;
            item.Unit = request.Unit.Trim();
            item.Location = request.Location.Trim();
            item.FoodCategoryId = request.FoodCategoryId ?? item.FoodCategoryId;
            item.PurchasedDate = request.PurchasedDate ?? item.PurchasedDate;
            item.MinimumQuantity = request.MinimumQuantity ?? item.MinimumQuantity;
            if (request.BestBeforeDate.HasValue &&
                (!item.BestBeforeDate.HasValue || request.BestBeforeDate < item.BestBeforeDate))
                item.BestBeforeDate = request.BestBeforeDate;
            item.UpdatedAt = DateTimeOffset.UtcNow;
        }
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
        if (isNew)
            await db.Entry(item).Reference(x => x.FoodProduct).LoadAsync(cancellationToken);
        if (item.FoodCategoryId.HasValue && item.FoodCategory is null)
            await db.Entry(item).Reference(x => x.FoodCategory).LoadAsync(cancellationToken);
        return isNew ? Results.Created($"/api/food-storage/stock/{item.Id}", ToDto(item)) : Results.Ok(ToDto(item));
    }

    private static async Task<IResult> UpdateStockItemAsync(int id, UpdateFoodStockItemRequest request,
        ApplicationDbContext db, CancellationToken cancellationToken)
    {
        var validation = ValidateStock(request.Quantity, request.Unit, request.Location);
        if (validation is not null) return Results.BadRequest(validation);
        if (request.MinimumQuantity is < 0) return Results.BadRequest("Minimum kan ikke være negativt.");
        if (request.FoodCategoryId is int categoryId &&
            !await db.FoodCategories.AnyAsync(x => x.Id == categoryId, cancellationToken))
            return Results.BadRequest("Kategorien finnes ikke.");
        var item = await db.FoodStockItems.Include(x => x.FoodProduct)
            .SingleOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (item is null) return Results.NotFound();

        item.Quantity = request.Quantity; item.Unit = request.Unit.Trim();
        item.Location = request.Location.Trim(); item.BestBeforeDate = request.BestBeforeDate;
        item.FoodCategoryId = request.FoodCategoryId;
        item.PurchasedDate = request.PurchasedDate; item.UpdatedAt = DateTimeOffset.UtcNow;
        item.MinimumQuantity = request.MinimumQuantity;
        await db.SaveChangesAsync(cancellationToken);
        return Results.Ok(ToDto(item));
    }

    private static async Task<IResult> DeleteStockItemAsync(int id, ApplicationDbContext db,
        CancellationToken cancellationToken)
    {
        var item = await db.FoodStockItems.SingleOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (item is null) return Results.NotFound();
        item.Quantity = 0;
        item.UpdatedAt = DateTimeOffset.UtcNow;
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

    private static readonly string[] DefaultCategoryNames =
        ["Kjøtt", "Meieri og melk", "Frukt og grønt", "Tørrvarer", "Drikke", "Frysevarer", "Annet"];

    private static async Task<IResult> GetCategoriesAsync(ApplicationDbContext db, ICurrentUser currentUser,
        CancellationToken cancellationToken)
    {
        var familyId = await currentUser.GetFamilyIdAsync(cancellationToken);
        if (familyId is null) return Results.Unauthorized();

        if (!await db.FoodCategories.AnyAsync(cancellationToken))
        {
            db.FoodCategories.AddRange(DefaultCategoryNames.Select(name => new FoodCategory
            {
                FamilyId = familyId.Value,
                Name = name
            }));
            await db.SaveChangesAsync(cancellationToken);
        }

        var categories = await db.FoodCategories.AsNoTracking().OrderBy(x => x.Name)
            .Select(x => new FoodCategoryDto(x.Id, x.Name)).ToListAsync(cancellationToken);
        return Results.Ok(categories);
    }

    private static async Task<IResult> CreateCategoryAsync(CreateFoodCategoryRequest request,
        ApplicationDbContext db, ICurrentUser currentUser, CancellationToken cancellationToken)
    {
        var familyId = await currentUser.GetFamilyIdAsync(cancellationToken);
        if (familyId is null) return Results.Unauthorized();
        var name = request.Name.Trim();
        if (name.Length is < 2 or > 100) return Results.BadRequest("Kategorinavn må inneholde mellom 2 og 100 tegn.");
        var existing = await db.FoodCategories.AsNoTracking().SingleOrDefaultAsync(x => x.Name == name, cancellationToken);
        if (existing is not null) return Results.Ok(new FoodCategoryDto(existing.Id, existing.Name));
        var category = new FoodCategory { FamilyId = familyId.Value, Name = name };
        db.FoodCategories.Add(category);
        await db.SaveChangesAsync(cancellationToken);
        return Results.Created($"/api/food-storage/categories/{category.Id}", new FoodCategoryDto(category.Id, category.Name));
    }

    private static async Task<IResult> SetMinimumQuantityAsync(int id, SetMinimumQuantityRequest request,
        ApplicationDbContext db, CancellationToken cancellationToken)
    {
        if (request.MinimumQuantity is < 0) return Results.BadRequest("Minimum kan ikke være negativt.");
        var item = await db.FoodStockItems.SingleOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (item is null) return Results.NotFound();
        item.MinimumQuantity = request.MinimumQuantity;
        item.UpdatedAt = DateTimeOffset.UtcNow;
        await db.SaveChangesAsync(cancellationToken);
        return Results.NoContent();
    }
    private static async Task<IResult> SetQuantityAsync(int id, SetStockQuantityRequest request,
        ApplicationDbContext db, CancellationToken cancellationToken)
    {
        if (request.Quantity < 0) return Results.BadRequest("Antall kan ikke være negativt.");
        var item = await db.FoodStockItems.SingleOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (item is null) return Results.NotFound();
        item.Quantity = request.Quantity;
        item.UpdatedAt = DateTimeOffset.UtcNow;
        await db.SaveChangesAsync(cancellationToken);
        return Results.NoContent();
    }

    private static async Task<IResult> WithdrawStockAsync(WithdrawFoodStockRequest request,
        ApplicationDbContext db, CancellationToken cancellationToken)
    {
        if (request.Quantity <= 0) return Results.BadRequest("Antall må være større enn null.");
        var item = await db.FoodStockItems.SingleOrDefaultAsync(
            x => x.FoodProductId == request.ProductId, cancellationToken);
        if (item is null) return Results.NotFound("Varen finnes ikke på lager.");
        if (request.Quantity > item.Quantity) return Results.BadRequest("Det er ikke nok av varen på lager.");

        item.Quantity -= request.Quantity;
        item.UpdatedAt = DateTimeOffset.UtcNow;
        db.FoodStockWithdrawals.Add(new FoodStockWithdrawal
        {
            FamilyId = item.FamilyId,
            FoodProductId = item.FoodProductId,
            Quantity = request.Quantity,
            RemainingQuantity = item.Quantity,
            Unit = item.Unit
        });
        await db.SaveChangesAsync(cancellationToken);
        return Results.NoContent();
    }

    private static async Task<IResult> GetStockWithdrawalsAsync(ApplicationDbContext db,
        CancellationToken cancellationToken)
    {
        var history = await db.FoodStockWithdrawals.AsNoTracking()
            .OrderByDescending(x => x.RemovedAt)
            .Take(200)
            .Select(x => new FoodStockWithdrawalDto(
                x.Id,
                x.FoodProductId,
                x.FoodProduct.Name,
                x.Quantity,
                x.RemainingQuantity,
                x.Unit,
                x.RemovedAt))
            .ToListAsync(cancellationToken);
        return Results.Ok(history);
    }



    private static async Task<IResult> GetShoppingListAsync(ApplicationDbContext db,
        CancellationToken cancellationToken)
    {
        var items = await db.FoodStockItems.AsNoTracking()
            .Where(x => x.MinimumQuantity.HasValue && x.Quantity < x.MinimumQuantity.Value)
            .OrderBy(x => x.FoodCategory!.Name).ThenBy(x => x.FoodProduct.Name)
            .Select(x => new FoodShoppingListItemDto(
                x.Id,
                x.FoodProduct.Name,
                x.FoodCategory == null ? null : x.FoodCategory.Name,
                x.Location,
                x.Quantity,
                x.MinimumQuantity!.Value,
                x.MinimumQuantity.Value - x.Quantity,
                x.Unit))
            .ToListAsync(cancellationToken);
        return Results.Ok(items);
    }

    private static FoodProductDto ToDto(FoodProduct product) => new(product.Id, product.Barcode,
        product.Name, product.Brand, product.QuantityLabel, product.ImageUrl, product.Source);
    private static FoodStockItemDto ToDto(FoodStockItem item, decimal? unitPrice = null) => new(item.Id, ToDto(item.FoodProduct),
        item.Quantity, item.Unit, item.Location, item.BestBeforeDate, item.PurchasedDate,
        item.FoodCategoryId, item.FoodCategory?.Name, unitPrice, unitPrice.GetValueOrDefault() * item.Quantity, item.MinimumQuantity);
    private static string NormalizeBarcode(string value) => new(value.Where(char.IsAsciiDigit).ToArray());
    private static string? Clean(string? value) => string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    private static string? ValidateStock(decimal quantity, string unit, string location) =>
        quantity <= 0 ? "Antall må være større enn null." :
        string.IsNullOrWhiteSpace(unit) ? "Enhet er påkrevd." :
        string.IsNullOrWhiteSpace(location) ? "Plassering er påkrevd." : null;
}
