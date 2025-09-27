namespace serviceApp.Server.Features.Vehicles;

public static class DeleteVehicle
{
    public record class Command(int Id) : ICommand<bool>;

    public class Handler(ApplicationDbContext context) : ICommandHandler<Command, bool>
    {
        private readonly ApplicationDbContext context = context;
        public async Task<Result<bool>> Handle(Command request, CancellationToken cancellationToken)
        {
            var vehicle = await context.Vehicles
                .Include(v => v.MileageHistories)
                .FirstOrDefaultAsync(v => v.Id == request.Id, cancellationToken);
            if (vehicle == null)
            {
                return Result.Fail<bool>($"Vehicle with ID {request.Id} not found.");
            }

            context.MileageHistories.RemoveRange(vehicle.MileageHistories);

            context.Vehicles.Remove(vehicle);
            await context.SaveChangesAsync(cancellationToken);
            return true;
        }
    }

    public class EndPoint : IEndpointDefinition
    {
        public void MapEndpoints(WebApplication app)
        {
            app.MapDelete("api/vehicle/{id}", async (ISender sender, int id, CancellationToken cancellationToken) =>
            {
                var result = await sender.Send(new Command(id), cancellationToken);
                if (result.Failure)
                {
                    return Results.NotFound(result.Error);
                }
                return Results.Ok(result.Value);
            }).RequireAuthorization(); ;
        }
    }

}


