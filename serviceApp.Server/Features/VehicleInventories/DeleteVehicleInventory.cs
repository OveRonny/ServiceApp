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
}

[ApiController]
[Route("api/vehicle-inventory")]
public class DeleteVehicleInventoryController(ISender sender) : ControllerBase
{
    private readonly ISender sender = sender;

    [HttpDelete("{id}")]
    public async Task<ActionResult<bool>> DeleteVehicleInventory(int id)
    {
        var result = await sender.Send(new DeleteVehicleInventory.Command(id));
        if (result.Failure)
        {
            return NotFound(result.Error);
        }
        return true;
    }
}
