namespace serviceApp.Server.Features.VehicleInventories;

public static class CreateVehicleInventory
{
    public record Command(string PartName, decimal Cost, string Description, int VehicleId, int SupplierId, int? QuantityInStock, int? ReorderThreshold) : ICommand<Response>;

    public record Response(int Id, string PartName, decimal Cost, string Description, int VehicleId, int SupplierId, DateTime PurchaseDate, int? QuantityInStock, int? ReorderThreshold);

    public class Handler(ApplicationDbContext context) : ICommandHandler<Command, Response>
    {
        private readonly ApplicationDbContext context = context;
        public async Task<Result<Response>> Handle(Command request, CancellationToken cancellationToken)
        {
            var vehicleInventory = new VehicleInventory
            {
                PartName = request.PartName,
                Cost = request.Cost,
                VehicleId = request.VehicleId,
                SupplierId = request.SupplierId,
                QuantityInStock = request.QuantityInStock,
                ReorderThreshold = request.ReorderThreshold,
                Description = request.Description,
                PurchaseDate = DateTime.Now
            };
            context.VehicleInventories.Add(vehicleInventory);
            await context.SaveChangesAsync(cancellationToken);
            return new Response(vehicleInventory.Id, vehicleInventory.PartName, vehicleInventory.Cost, vehicleInventory.Description,
                vehicleInventory.VehicleId, vehicleInventory.SupplierId, vehicleInventory.PurchaseDate,
                vehicleInventory.QuantityInStock, vehicleInventory.ReorderThreshold);
        }
    }
}

[ApiController]
[Route("api/vehicle-inventory")]
public class CreateVehicleInventoryController(ISender sender) : ControllerBase
{
    private readonly ISender sender = sender;

    [HttpPost]
    public async Task<ActionResult<CreateVehicleInventory.Response>> CreateVehicleInventory([FromBody] CreateVehicleInventory.Command command)
    {
        var result = await sender.Send(command);
        return Ok(result.Value);
    }
}
