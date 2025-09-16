namespace serviceApp.Server.Features.Vehicles;

public static class GetvehicleById
{
    public record class Query(int Id) : IQuery<Response>;
    public record Response(int Id, int OwnerId, string Make, string Model, string Year, string Color,
        string LicensePlate, DateTime DateCreated);
    public class Handler(ApplicationDbContext context) : IQueryHandler<Query, Response>
    {
        private readonly ApplicationDbContext context = context;
        public async Task<Result<Response>> Handle(Query request, CancellationToken cancellationToken)
        {
            var vehicle = await context.Vehicles
            .AsNoTracking()
            .FirstOrDefaultAsync(v => v.Id == request.Id, cancellationToken);
            if (vehicle == null)
            {
                return Result.Fail<Response>($"Vehicle with ID {request.Id} not found.");
            }
            return new Response(vehicle.Id, vehicle.OwnerId, vehicle.Make, vehicle.Model, vehicle.Year, vehicle.Color, vehicle.LicensePlate, vehicle.DateCreated);
        }
    }

    public class EndPoint : IEndpointDefinition
    {
        public void MapEndpoints(WebApplication app)
        {
            app.MapGet("api/vehicle/{id}", async (ISender sender, int id, CancellationToken cancellationToken) =>
            {
                var result = await sender.Send(new Query(id), cancellationToken);
                if (result.Failure)
                {
                    return Results.NotFound(result.Error);
                }
                return Results.Ok(result.Value);
            })
                .RequireAuthorization();

        }
    }
}




