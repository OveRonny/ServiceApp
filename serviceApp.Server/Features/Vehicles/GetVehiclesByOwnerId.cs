namespace serviceApp.Server.Features.Vehicles;

public static class GetVehiclesByOwnerId
{
    public record Query(int OwnerId) : IQuery<List<Response>>;

    public record Response(int Id, int OwnerId, string Make, string Model, int Year, string Color,
        string LicensePlate, DateTime DateCreated);

    public class Handler(ApplicationDbContext context) : IQueryHandler<Query, List<Response>>
    {
        private readonly ApplicationDbContext context = context;

        public async Task<Result<List<Response>>> Handle(Query request, CancellationToken cancellationToken)
        {
            var vehicles = await context.Vehicles
                .Where(v => v.OwnerId == request.OwnerId)
                .OrderByDescending(v => v.DateCreated)
                .Select(v => new Response(v.Id, v.OwnerId, v.Make, v.Model, v.Year, v.Color, v.LicensePlate, v.DateCreated))
                .ToListAsync(cancellationToken);

            return Result.Ok(vehicles);
        }
    }

    public class EndPoint : IEndpointDefinition
    {
        public void MapEndpoints(WebApplication app)
        {
            app.MapGet("api/vehicle/owner/{ownerId:int}", async (ISender sender, int ownerId, CancellationToken ct) =>
            {
                var result = await sender.Send(new Query(ownerId), ct);
                if (result.Failure) return Results.NotFound(result.Error);
                return Results.Ok(result.Value);
            });
        }
    }
}