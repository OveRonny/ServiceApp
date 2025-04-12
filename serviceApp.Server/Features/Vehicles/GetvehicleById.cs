namespace serviceApp.Server.Features.Vehicles;

public static class GetvehicleById
{
    public record class Query(int Id) : IQuery<Response>;
    public record Response(int Id, int OwnerId, string Make, string Model, string Year, string Color,
        string LicensePlate, DateTime DateCreated);
    public class Handler(ApplicationDbContext context) : IQueryHandler<Query, Response>
    {
        private readonly ApplicationDbContext context = context;
        public async Task<Result<Response>> Handle(Query request, CancellationToken cancellationToken)
        {
            var vehicle = await context.Vehicles.FindAsync(request.Id);
            if (vehicle == null)
            {
                return Result.Fail<Response>($"Vehicle with ID {request.Id} not found.");
            }
            return new Response(vehicle.Id, vehicle.OwnerId, vehicle.Make, vehicle.Model, vehicle.Year, vehicle.Color, vehicle.LicensePlate, vehicle.DateCreated);
        }
    }
}

[ApiController]
[Route("api/vehicle")]
public class GetVehicleByIdController(ISender sender) : ControllerBase
{
    private readonly ISender sender = sender;

    [HttpGet("{id}")]
    public async Task<ActionResult<GetvehicleById.Response>> GetVehicleById(int id)
    {
        var result = await sender.Send(new GetvehicleById.Query(id));
        if (result.Failure)
        {
            return NotFound(result.Error);
        }
        return Ok(result.Value);
    }
}


