namespace serviceApp.Server.Features.Vehicles;

public static class CreateVehicle
{
    public record Command(string Make, string Model, string Year, string Color, string LicensePlate, int OwnerId) : ICommand<Response>;

    public record Response(int Id, string Make, string Model, string Year, string Color, string LicensePlate, int OwnerId, DateTime DateCreated);

    public class Handler(ApplicationDbContext context) : ICommandHandler<Command, Response>
    {
        private readonly ApplicationDbContext context = context;

        public async Task<Result<Response>> Handle(Command request, CancellationToken cancellationToken)
        {
            var vehicle = new Vehicle
            {
                Make = request.Make,
                Model = request.Model,
                Year = request.Year,
                Color = request.Color,
                LicensePlate = request.LicensePlate,
                DateCreated = DateTime.Now,
                OwnerId = request.OwnerId
            };
            context.Vehicles.Add(vehicle);
            await context.SaveChangesAsync(cancellationToken);
            return new Response(vehicle.Id, vehicle.Make, vehicle.Model, vehicle.Year, vehicle.Color, vehicle.LicensePlate, vehicle.OwnerId, vehicle.DateCreated);
        }
    }

    public class EndPoint : IEndpointDefinition
    {
        public void MapEndpoints(WebApplication app)
        {
            app.MapPost("api/vehicle", async (ISender sender, CreateVehicle.Command command, CancellationToken cancellationToken) =>
            {
                var result = await sender.Send(command, cancellationToken);
                return Results.Ok(result.Value);
            });
        }
    }

}



