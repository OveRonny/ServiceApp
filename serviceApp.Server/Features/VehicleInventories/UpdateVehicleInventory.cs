namespace serviceApp.Server.Features.VehicleInventories;

public static class UpdateVehicleInventory
{
    public record Command(int Id, string PartName, decimal Cost, string Description, int VehicleId, int SupplierId, decimal? QuantityInStock, decimal? ReorderThreshold, UnitOfMeasure Unit = UnitOfMeasure.Piece,
        decimal? QuantityDelta = null) : ICommand<Response>;

    public record Response(int Id, string PartName, decimal Cost, string Description, int VehicleId, int SupplierId, DateTime PurchaseDate, decimal? QuantityInStock, decimal? ReorderThreshold, UnitOfMeasure Unit);

    public class Handler(ApplicationDbContext context) : ICommandHandler<Command, Response>
    {
        private readonly ApplicationDbContext context = context;

        public async Task<Result<Response>> Handle(Command request, CancellationToken cancellationToken)
        {
            var vehicleInventory = await context.VehicleInventories
                .FirstOrDefaultAsync(v => v.Id == request.Id, cancellationToken);

            if (vehicleInventory is null)
                return Result.Fail<Response>("Vehicle inventory not found");

            // Update descriptive fields
            vehicleInventory.PartName = request.PartName;
            vehicleInventory.Description = request.Description;
            vehicleInventory.VehicleId = request.VehicleId;
            vehicleInventory.SupplierId = request.SupplierId;
            vehicleInventory.ReorderThreshold = request.ReorderThreshold;
            vehicleInventory.Unit = request.Unit;

            // Stock handling:
            // 1) If QuantityDelta provided -> adjust by delta; weighted-average Cost if adding stock.
            // 2) Else if QuantityInStock provided -> set absolute quantity (no cost averaging).
            if (request.QuantityDelta.HasValue)
            {
                var oldQty = vehicleInventory.QuantityInStock ?? 0m;
                var delta = request.QuantityDelta.Value;
                var newQty = oldQty + delta;

                if (newQty < 0m)
                    return Result.Fail<Response>($"Resulting stock would be negative ({newQty}).");

                // If adding stock, compute weighted-average unit cost using the provided Cost as the incoming unit price.
                if (delta > 0m)
                {
                    var oldCost = vehicleInventory.Cost;
                    var totalQty = oldQty + delta;
                    if (totalQty > 0m)
                    {
                        vehicleInventory.Cost = Math.Round(((oldQty * oldCost) + (delta * request.Cost)) / totalQty, 2);
                    }

                    vehicleInventory.PurchaseDate = DateTime.Now; // reflect latest purchase
                }

                // If delta < 0, we don't change unit cost; just reduce stock.
                vehicleInventory.QuantityInStock = newQty;
            }
            else if (request.QuantityInStock.HasValue)
            {
                // Absolute set of quantity
                if (request.QuantityInStock.Value < 0m)
                    return Result.Fail<Response>("QuantityInStock cannot be negative.");

                vehicleInventory.QuantityInStock = request.QuantityInStock;
                // Update unit cost directly from request (treat as an edit)
                vehicleInventory.Cost = Math.Round(request.Cost, 2);
                // Do not touch PurchaseDate unless you want to mark manual edits as "now"
            }
            else
            {
                // No stock change; still allow editing unit cost if desired
                vehicleInventory.Cost = Math.Round(request.Cost, 2);
            }

            try
            {
                await context.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                return Result.Fail<Response>("The inventory item was modified by another request. Please reload and try again.");
            }

            return new Response(
                vehicleInventory.Id,
                vehicleInventory.PartName,
                vehicleInventory.Cost,
                vehicleInventory.Description,
                vehicleInventory.VehicleId,
                vehicleInventory.SupplierId,
                vehicleInventory.PurchaseDate,
                vehicleInventory.QuantityInStock,
                vehicleInventory.ReorderThreshold,
                vehicleInventory.Unit
            );
        }
    }

    public class EndPoint : IEndpointDefinition
    {
        public void MapEndpoints(WebApplication app)
        {
            app.MapPut("api/vehicle-inventory/{id}", async (ISender sender, int id, Command command, CancellationToken cancellationToken) =>
            {
                if (id != command.Id)
                {
                    return Results.BadRequest("ID mismatch");
                }
                var result = await sender.Send(command, cancellationToken);
                if (result.Failure)
                {
                    return Results.NotFound(result.Error);
                }
                return Results.Ok(result.Value);
            });
        }
    }
}


