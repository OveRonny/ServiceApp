namespace serviceApp.Server.Features.InsurancePolicies;

public static class GetRemainingMileage
{
    public record class Query(int VehicleId) : IQuery<Response>;
    public record Response(int RemainingMileage, DateTime? EnDate);
    public class Handler(ApplicationDbContext context) : IQueryHandler<Query, Response>
    {
        private readonly ApplicationDbContext context = context;
        public async Task<Result<Response>> Handle(Query request, CancellationToken cancellationToken)
        {

            var vehicleExists = await context.Vehicles.AnyAsync(v => v.Id == request.VehicleId, cancellationToken);
            if (!vehicleExists)
            {
                return Result.Fail<Response>($"Vehicle with ID {request.VehicleId} does not exist.");
            }


            var insurancePolicy = await context.InsurancePolicies
                .FirstOrDefaultAsync(p => p.VehicleId == request.VehicleId && p.IsActive, cancellationToken);
            if (insurancePolicy == null)
            {
                return Result.Fail<Response>($"No active insurance policy found for vehicle ID {request.VehicleId}.");
            }


            var mileageHistories = await context.MileageHistories
                .Where(m => m.VehicleId == request.VehicleId &&
                            m.RecordedDate >= insurancePolicy.RenewalDate &&
                            m.RecordedDate <= (insurancePolicy.EndDate ?? DateTime.UtcNow))
                .ToListAsync(cancellationToken);


            int remainingMileage = insurancePolicy.CalculateRemainingMileage(mileageHistories);

            return Result.Ok(new Response(remainingMileage, insurancePolicy.EndDate));
        }
    }

    public class EndPoint : IEndpointDefinition
    {
        public void MapEndpoints(WebApplication app)
        {
            app.MapGet("api/insurance/remaining-mileage/{vehicleId}", async (ISender sender, int vehicleId, CancellationToken cancellationToken) =>
            {
                var result = await sender.Send(new Query(vehicleId), cancellationToken);
                if (result.Failure)
                {
                    return Results.NotFound(result.Error);
                }
                return Results.Ok(result.Value);
            });
        }
    }
}

//[ApiController]
//[Route("api/insurance")]
//public class GetRemainingMileageController(ISender sender) : ControllerBase
//{
//    private readonly ISender sender = sender;

//    [HttpGet("remaining-mileage/{vehicleId}")]
//    public async Task<ActionResult<GetRemainingMileage.Response>> GetRemainingMileage(int vehicleId)
//    {
//        var result = await sender.Send(new GetRemainingMileage.Query(vehicleId));
//        if (result.Failure)
//        {
//            return NotFound(result.Error);
//        }
//        return Ok(result.Value);
//    }
//}
