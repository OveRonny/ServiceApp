namespace serviceApp.Server.Features.VehicleInventories;

public static class GetVehicleInventoryById
{
    public record class Query(int Id) : IQuery<Response>;
    public record Response(int Id, string PartName, decimal Cost, string Description, int VehicleId, int SupplierId,
        DateTime PurchaseDate, decimal? QuantityInStock, decimal? ReorderThreshold);

    public class Handler(ApplicationDbContext context) : IQueryHandler<Query, Response>
    {
        private readonly ApplicationDbContext context = context;
        public async Task<Result<Response>> Handle(Query request, CancellationToken cancellationToken)
        {
            var vehicleInventory = await context.VehicleInventories.FindAsync(request.Id);
            if (vehicleInventory == null)
            {
                return Result.Fail<Response>($"Vehicle inventory with ID {request.Id} not found.");
            }
            var response = new Response(vehicleInventory.Id, vehicleInventory.PartName, vehicleInventory.Cost,
                vehicleInventory.Description, vehicleInventory.VehicleId, vehicleInventory.SupplierId,
                vehicleInventory.PurchaseDate, vehicleInventory.QuantityInStock, vehicleInventory.ReorderThreshold);
            return Result.Ok(response);
        }
    }

    public class EndPoint : IEndpointDefinition
    {
        public void MapEndpoints(WebApplication app)
        {
            app.MapGet("api/vehicle-inventory/{id}", async (ISender sender, int id, CancellationToken cancellationToken) =>
            {
                var result = await sender.Send(new Query(id), cancellationToken);
                if (result.Failure)
                {
                    return Results.NotFound(result.Error);
                }
                return Results.Ok(result.Value);
            }).RequireAuthorization(); ;
        }
    }
}



