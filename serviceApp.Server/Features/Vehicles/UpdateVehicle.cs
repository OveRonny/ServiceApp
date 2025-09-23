namespace serviceApp.Server.Features.Vehicles;

public static class UpdateVehicle
{
    public record class Command(int Id, string Make, string Model, int Year, string Color, int OwnerId, string LicensePlate) : ICommand<Response>;
    public record Response(int Id, string Make, string Model, int Year, string Color, string LicensePlate, int OwnerId, DateTime DateCreated);

    public class Handler(ApplicationDbContext context) : ICommandHandler<Command, Response>
    {
        private readonly ApplicationDbContext context = context;
        public async Task<Result<Response>> Handle(Command request, CancellationToken cancellationToken)
        {
            var vehicle = await context.Vehicles.FindAsync(request.Id);
            if (vehicle == null)
            {
                return Result.Fail<Response>($"Vehicle with ID {request.Id} not found.");
            }
            vehicle.Make = request.Make;
            vehicle.Model = request.Model;
            vehicle.Year = request.Year;
            vehicle.Color = request.Color;
            vehicle.LicensePlate = request.LicensePlate;
            vehicle.OwnerId = request.OwnerId;
            await context.SaveChangesAsync(cancellationToken);
            return new Response(vehicle.Id, vehicle.Make, vehicle.Model, vehicle.Year, vehicle.Color, vehicle.LicensePlate, vehicle.OwnerId, vehicle.DateCreated);
        }
    }

    public class EndPoint : IEndpointDefinition
    {
        public void MapEndpoints(WebApplication app)
        {
            app.MapPut("api/vehicle/{id}", async (ISender sender, int id, UpdateVehicle.Command command, CancellationToken cancellationToken) =>
            {
                if (id != command.Id)
                {
                    return Results.BadRequest("ID in the URL does not match ID in the request body.");
                }
                var result = await sender.Send(command, cancellationToken);
                if (result.Failure)
                {
                    return Results.NotFound(result.Error);
                }
                return Results.Ok(result.Value);
            }).RequireAuthorization(); ;
        }
    }
}


