namespace serviceApp.Server.Features.VehicleInventories;

public static class UpdateVehicleInventory
{
    public record Command(int Id, string PartName, decimal Cost, string Description, int VehicleId, int SupplierId, int? QuantityInStock, int? ReorderThreshold) : ICommand<Response>;

    public record Response(int Id, string PartName, decimal Cost, string Description, int VehicleId, int SupplierId, DateTime PurchaseDate, int? QuantityInStock, int? ReorderThreshold);

    public class Handler(ApplicationDbContext context) : ICommandHandler<Command, Response>
    {
        private readonly ApplicationDbContext context = context;
        public async Task<Result<Response>> Handle(Command request, CancellationToken cancellationToken)
        {
            var vehicleInventory = await context.VehicleInventories.FindAsync(request.Id);

            if (vehicleInventory == null)
            {
                return Result.Fail<Response>("Vehicle inventory not found");
            }
            vehicleInventory.PartName = request.PartName;
            vehicleInventory.Cost = request.Cost;
            vehicleInventory.Description = request.Description;
            vehicleInventory.VehicleId = request.VehicleId;
            vehicleInventory.SupplierId = request.SupplierId;
            vehicleInventory.QuantityInStock = request.QuantityInStock;
            vehicleInventory.ReorderThreshold = request.ReorderThreshold;

            context.VehicleInventories.Update(vehicleInventory);
            await context.SaveChangesAsync(cancellationToken);
            return new Response(vehicleInventory.Id, vehicleInventory.PartName, vehicleInventory.Cost, vehicleInventory.Description,
                vehicleInventory.VehicleId, vehicleInventory.SupplierId, vehicleInventory.PurchaseDate,
                vehicleInventory.QuantityInStock, vehicleInventory.ReorderThreshold);
        }
    }
}

[ApiController]
[Route("api/vehicle-inventory")]
public class UpdateVehicleInventoryController(ISender sender) : ControllerBase
{
    private readonly ISender sender = sender;

    [HttpPut("{id}")]
    public async Task<ActionResult<UpdateVehicleInventory.Response>> UpdateVehicleInventory(int id, [FromBody] UpdateVehicleInventory.Command command)
    {
        if (id != command.Id)
        {
            return BadRequest("ID mismatch");
        }
        var result = await sender.Send(command);
        if (result.Failure)
        {
            return NotFound(result.Error);
        }
        return Ok(result.Value);
    }
}
