namespace serviceApp.Server.Features.VehicleInventories;

public static class DeleteVehicleInventory
{
    public record Command(int Id) : ICommand<bool>;
    public class Handler(ApplicationDbContext context) : ICommandHandler<Command, bool>
    {
        private readonly ApplicationDbContext context = context;
        public async Task<Result<bool>> Handle(Command request, CancellationToken cancellationToken)
        {
            var vehicleInventory = await context.VehicleInventories.FindAsync(request.Id);
            if (vehicleInventory == null)
            {
                return false;
            }
            context.VehicleInventories.Remove(vehicleInventory);
            await context.SaveChangesAsync(cancellationToken);
            return true;
        }
    }

    public class EndPoint : IEndpointDefinition
    {
        public void MapEndpoints(WebApplication app)
        {
            app.MapDelete("api/vehicle-inventory/{id}", async (ISender sender, int id, CancellationToken cancellationToken) =>
            {
                var result = await sender.Send(new Command(id), cancellationToken);
                if (result.Failure)
                {
                    return Results.NotFound(result.Error);
                }
                return Results.Ok(true);
            });
        }
    }
}


