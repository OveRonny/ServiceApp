namespace serviceApp.Server.Features.InsurancePolicies;

public static class GetAllIInsurance
{
    public record class Query : IQuery<Response>;
    public record Response(List<InsurancePolicyDto> InsurancePolicies);
    public record InsurancePolicyDto(int Id, string CompanyName, decimal AnnualPrice, int AnnualMileageLimit,
        int VehicleId, DateTime RenewalDate, int StartingMileage, bool IsActive, DateTime? EndDate);
    public class Handler(ApplicationDbContext context) : IQueryHandler<Query, Response>
    {
        private readonly ApplicationDbContext context = context;
        public async Task<Result<Response>> Handle(Query request, CancellationToken cancellationToken)
        {
            var insurancePolicies = await context.InsurancePolicies
                .Include(i => i.Vehicle)
                .Where(i => i.IsActive)
                .ToListAsync(cancellationToken);

            var insurancePolicyDtos = insurancePolicies.Select(i => new InsurancePolicyDto(
                i.Id,
                i.CompanyName,
                i.AnnualPrice,
                i.AnnualMileageLimit,
                i.VehicleId,
                i.RenewalDate,
                i.StartingMileage,
                i.IsActive,
                i.EndDate
            )).ToList();

            return Result.Ok(new Response(insurancePolicyDtos));
        }
    }

    public class EndPoint : IEndpointDefinition
    {
        public void MapEndpoints(WebApplication app)
        {
            app.MapGet("api/insurance", async (ISender sender, CancellationToken cancellationToken) =>
            {
                var result = await sender.Send(new Query(), cancellationToken);
                return Results.Ok(result.Value);
            });
        }
    }
}

//[ApiController]
//[Route("api/insurance")]
//public class GetAllIInsuranceController(ISender sender) : ControllerBase
//{
//    private readonly ISender sender = sender;

//    [HttpGet]
//    public async Task<ActionResult<GetAllIInsurance.Response>> GetAllIInsurance()
//    {
//        var result = await sender.Send(new GetAllIInsurance.Query());
//        if (result.Failure)
//        {
//            return NotFound(result.Error);
//        }
//        return Ok(result.Value);
//    }
//}
