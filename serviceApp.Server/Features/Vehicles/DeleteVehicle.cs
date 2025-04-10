using Microsoft.AspNetCore.Mvc;
using serviceApp.Server.Abstractions;
using serviceApp.Server.Abstractions.RequestHandling;
using serviceApp.Server.Data;

namespace serviceApp.Server.Features.Vehicles;

public static class DeleteVehicle
{
    public record class Command(int Id) : ICommand<bool>;

    public class Handler(ApplicationDbContext context) : ICommandHandler<Command, bool>
    {
        private readonly ApplicationDbContext context = context;
        public async Task<Result<bool>> Handle(Command request, CancellationToken cancellationToken)
        {
            var vehicle = await context.Vehicles.FindAsync(request.Id);
            if (vehicle == null)
            {
                return Result.Fail<bool>($"Vehicle with ID {request.Id} not found.");
            }
            context.Vehicles.Remove(vehicle);
            await context.SaveChangesAsync(cancellationToken);
            return true;
        }
    }

}

[ApiController]
[Route("api/vehicle")]
public class DeleteVehicleController(ISender sender) : ControllerBase
{
    private readonly ISender sender = sender;

    [HttpDelete("{id}")]
    public async Task<ActionResult<bool>> DeleteVehicle(int id)
    {
        var result = await sender.Send(new DeleteVehicle.Command(id));
        if (result.Failure)
        {
            return NotFound(result.Error);
        }
        return true;
    }
}
