namespace serviceApp.Server.Features.VehicleInventories;

public static class GetAllVehicleInventory
{
    public record Query : IQuery<List<Response>>;
    public record Response(int Id, string PartName, decimal Cost, string Description, int VehicleId, int SupplierId,
        DateTime PurchaseDate, int? QuantityInStock, int? ReorderThreshold);

    public class Handler(ApplicationDbContext context) : IQueryHandler<Query, List<Response>>
    {
        private readonly ApplicationDbContext context = context;
        public async Task<Result<List<Response>>> Handle(Query request, CancellationToken cancellationToken)
        {
            var vehicleInventories = await context.VehicleInventories.ToListAsync(cancellationToken);
            var response = vehicleInventories.Select(vi => new Response(vi.Id, vi.PartName, vi.Cost, vi.Description,
                vi.VehicleId, vi.SupplierId, vi.PurchaseDate, vi.QuantityInStock, vi.ReorderThreshold)).ToList();
            return Result.Ok(response);
        }
    }
}

[ApiController]
[Route("api/vehicle-inventory")]
public class GetAllVehicleInventoryController(ISender sender) : ControllerBase
{
    private readonly ISender sender = sender;

    [HttpGet]
    public async Task<ActionResult<List<GetAllVehicleInventory.Response>>> GetAllVehicleInventories()
    {
        var result = await sender.Send(new GetAllVehicleInventory.Query());
        return Ok(result);
    }
}
