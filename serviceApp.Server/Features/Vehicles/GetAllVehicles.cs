namespace serviceApp.Server.Features.Vehicles;

public static class GetAllVehicles
{
    public record Query : IQuery<List<Response>>;

    public record Response(int Id, string Make, string Model, string Year,
        string Color, string LicensePlate, DateTime DateCreated, string FirstName);

    public class Handler(ApplicationDbContext context) : IQueryHandler<Query, List<Response>>
    {
        private readonly ApplicationDbContext context = context;
        public async Task<Result<List<Response>>> Handle(Query request, CancellationToken cancellationToken)
        {
            var vehicles = await context.Vehicles
                .Include(v => v.Owner)
                .ToListAsync(cancellationToken);
            var response = vehicles.Select(v => new Response(v.Id, v.Make, v.Model, v.Year,
                v.Color, v.LicensePlate, v.DateCreated, v.Owner!.FirstName)).ToList();
            return Result.Ok(response);
        }
    }
}

[ApiController]
[Route("api/vehicle")]
public class GetAllVehicleController(ISender sender) : ControllerBase
{
    private readonly ISender sender = sender;

    [HttpGet]
    public async Task<ActionResult<List<GetAllVehicles.Response>>> GetAllVehicles()
    {
        var result = await sender.Send(new GetAllVehicles.Query());
        return Ok(result.Value);
    }
}
