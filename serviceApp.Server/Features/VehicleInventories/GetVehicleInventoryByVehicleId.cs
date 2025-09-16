namespace serviceApp.Server.Features.VehicleInventories;

public static class GetVehicleInventoryByVehicleId
{
    public record class Query(int VehicleId) : IQuery<List<Response>>;

    public record Response(int Id, string PartName, decimal Cost, string Description, int VehicleId, int SupplierId,
        DateTime PurchaseDate, int? QuantityInStock, int? ReorderThreshold);

    public class Handler(ApplicationDbContext context) : IQueryHandler<Query, List<Response>>
    {
        private readonly ApplicationDbContext context = context;
        public async Task<Result<List<Response>>> Handle(Query request, CancellationToken cancellationToken)
        {
            var vehicleInventories = await context.VehicleInventories
                .Where(vi => vi.VehicleId == request.VehicleId)
                .ToListAsync(cancellationToken);
            var response = vehicleInventories.Select(vi => new Response(vi.Id, vi.PartName, vi.Cost, vi.Description,
                vi.VehicleId, vi.SupplierId, vi.PurchaseDate, vi.QuantityInStock, vi.ReorderThreshold)).ToList();
            return Result.Ok(response);
        }
    }

    public class EndPoint : IEndpointDefinition
    {
        public void MapEndpoints(WebApplication app)
        {
            app.MapGet("api/vehicle-inventory/vehicle/{vehicleId}", async (ISender sender, int vehicleId, CancellationToken cancellationToken) =>
            {
                var result = await sender.Send(new Query(vehicleId), cancellationToken);
                return Results.Ok(result.Value);
            });
        }
    }
}
