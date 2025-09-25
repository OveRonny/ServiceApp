using serviceApp.Server.Features.Autentication;

namespace serviceApp.Server.Features.VehicleInventories;

public static class CreateVehicleInventory
{
    public record Command(string PartName, decimal Cost, string Description, int VehicleId, int SupplierId, decimal? QuantityInStock, decimal? ReorderThreshold, UnitOfMeasure Unit = UnitOfMeasure.Piece,
        bool UpsertIfExists = true) : ICommand<Response>;

    public record Response(int Id, string PartName, decimal Cost, string Description, int VehicleId, int SupplierId, DateTime PurchaseDate, decimal? QuantityInStock, decimal? ReorderThreshold, UnitOfMeasure Unit);

    public class Handler(ApplicationDbContext context, ICurrentUser currentUser) : ICommandHandler<Command, Response>
    {
        private readonly ApplicationDbContext context = context;
        private readonly ICurrentUser currentUser = currentUser;

        public async Task<Result<Response>> Handle(Command request, CancellationToken cancellationToken)
        {

            var familyId = await currentUser.GetFamilyIdAsync(cancellationToken);
            if (familyId is null)
                return Result.Fail<Response>("Not authenticated.");

            // Normalize quantity (supports decimals for things like oil in liters)
            var qtyToAdd = request.QuantityInStock ?? 0m;

            // Optional upsert: if an inventory item with same Vehicle + PartName + Unit exists, increment stock
            if (request.UpsertIfExists)
            {
                var existing = await context.VehicleInventories
                    .FirstOrDefaultAsync(v =>
                        v.VehicleId == request.VehicleId &&
                        v.PartName == request.PartName &&
                        v.Unit == request.Unit,
                        cancellationToken);

                if (existing is not null)
                {
                    var oldQty = existing.QuantityInStock ?? 0m;
                    var newQty = oldQty + qtyToAdd;

                    // Weighted-average unit cost (all prices already include VAT in your scenario)
                    if (newQty > 0)
                    {
                        existing.Cost = Math.Round(
                            ((oldQty * existing.Cost) + (qtyToAdd * request.Cost)) / newQty, 2);
                    }

                    existing.QuantityInStock = newQty;
                    existing.PurchaseDate = DateTime.Now;
                    existing.SupplierId = request.SupplierId;
                    // Keep existing description if set; otherwise take the new one
                    if (string.IsNullOrWhiteSpace(existing.Description))
                        existing.Description = request.Description;

                    await context.SaveChangesAsync(cancellationToken);

                    return new Response(
                        existing.Id,
                        existing.PartName,
                        existing.Cost,
                        existing.Description,
                        existing.VehicleId,
                        existing.SupplierId,
                        existing.PurchaseDate,
                        existing.QuantityInStock,
                        existing.ReorderThreshold,
                        existing.Unit
                    );
                }
            }

            var vehicleInventory = new VehicleInventory
            {
                PartName = request.PartName,
                Cost = request.Cost,
                VehicleId = request.VehicleId,
                SupplierId = request.SupplierId,
                QuantityInStock = request.QuantityInStock,
                ReorderThreshold = request.ReorderThreshold,
                Description = request.Description,
                PurchaseDate = DateTime.Now,
                Unit = request.Unit
            };
            context.VehicleInventories.Add(vehicleInventory);
            await context.SaveChangesAsync(cancellationToken);
            return new Response(vehicleInventory.Id, vehicleInventory.PartName, vehicleInventory.Cost, vehicleInventory.Description,
                vehicleInventory.VehicleId, vehicleInventory.SupplierId, vehicleInventory.PurchaseDate,
                vehicleInventory.QuantityInStock, vehicleInventory.ReorderThreshold, vehicleInventory.Unit);
        }
    }

    public class EndPoint : IEndpointDefinition
    {
        public void MapEndpoints(WebApplication app)
        {
            app.MapPost("api/vehicle-inventory", async (ISender sender, Command command, CancellationToken cancellationToken) =>
            {
                var result = await sender.Send(command, cancellationToken);
                if (result.Failure)
                {
                    return Results.BadRequest(result.Error);
                }
                return Results.Ok(result.Value);
            }).RequireAuthorization(); ;
        }
    }
}


