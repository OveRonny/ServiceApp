using serviceApp.Server.Features.Autentication;

namespace serviceApp.Server.Features.Vehicles;

public static class CreateVehicle
{
    public record Command(string Make, string Model, int Year, string Color, string LicensePlate, int OwnerId) : ICommand<Response>;

    public record Response(int Id, string Make, string Model, int Year, string Color, string LicensePlate, int OwnerId, DateTime DateCreated);

    public class Handler(ApplicationDbContext context, ICurrentUser currentUser) : ICommandHandler<Command, Response>
    {
        private readonly ApplicationDbContext context = context;
        private readonly ICurrentUser currentUser = currentUser;

        public async Task<Result<Response>> Handle(Command request, CancellationToken cancellationToken)
        {

            var familyId = await currentUser.GetFamilyIdAsync(cancellationToken);
            if (familyId is null)
                return Result.Fail<Response>("Not authenticated.");

            var vehicle = new Vehicle
            {
                Make = request.Make,
                Model = request.Model,
                Year = request.Year,
                Color = request.Color,
                LicensePlate = request.LicensePlate,
                DateCreated = DateTime.Now,
                OwnerId = request.OwnerId,
                UserId = currentUser.UserId!
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
            app.MapPost("api/vehicle", async (ISender sender, Command command, CancellationToken cancellationToken) =>
            {
                var result = await sender.Send(command, cancellationToken);
                return result.Failure ? Results.Unauthorized() : Results.Ok(result.Value);
            })
            .RequireAuthorization();
        }
    }

}



