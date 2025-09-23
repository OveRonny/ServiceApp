namespace serviceApp.Server.Features.VehicleInventories;

public static class GetVehicleInventoryByVehicleId
{
    public record class Query(int VehicleId, bool IncludeZero = true) : IQuery<List<Response>>;

    public record Response(int Id, string PartName, decimal Cost, string Description, int VehicleId, int SupplierId,
        DateTime PurchaseDate, decimal? QuantityInStock, decimal? ReorderThreshold);

    public class Handler(ApplicationDbContext context) : IQueryHandler<Query, List<Response>>
    {
        private readonly ApplicationDbContext context = context;
        public async Task<Result<List<Response>>> Handle(Query request, CancellationToken cancellationToken)
        {
            // Filter out items with null or zero stock
            var response = context.VehicleInventories
                .AsNoTracking()
                .Where(vi => vi.VehicleId == request.VehicleId);

            if (!request.IncludeZero)
            {
                response = response.Where(vi => (vi.QuantityInStock ?? 0m) > 0m);
            }

            var result = await response
              .Select(vi => new Response(
                  vi.Id,
                  vi.PartName,
                  vi.Cost,
                  vi.Description,
                  vi.VehicleId,
                  vi.SupplierId,
                  vi.PurchaseDate,
                  vi.QuantityInStock,
                  vi.ReorderThreshold))
              .ToListAsync(cancellationToken);

            return Result.Ok(result);
        }
    }

    public class EndPoint : IEndpointDefinition
    {
        public void MapEndpoints(WebApplication app)
        {
            app.MapGet("api/vehicle-inventory/vehicle/{vehicleId}", async (ISender sender, int vehicleId, bool? includeZero, CancellationToken cancellationToken) =>
            {
                var result = await sender.Send(new Query(vehicleId, includeZero ?? true), cancellationToken);
                return Results.Ok(result.Value);
            }).RequireAuthorization(); ;
        }
    }
}
