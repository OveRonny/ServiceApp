namespace serviceApp.Server.Features.InsurancePolicies;

public static class CreateInsurance
{
    public record Command(string CompanyName, decimal AnnualPrice, int AnnualMileageLimit, int VehicleId,
        DateTime RenewalDate, int StartingMileage) : ICommand<Response>;

    public record Response(int Id, string CompanyName, decimal AnnualPrice, int AnnualMileageLimit, int VehicleId,
        DateTime RenewalDate, int StartingMileage, bool IsActive, DateTime? EndDate);

    public class Handler(ApplicationDbContext context) : ICommandHandler<Command, Response>
    {
        private readonly ApplicationDbContext context = context;

        public async Task<Result<Response>> Handle(Command request, CancellationToken cancellationToken)
        {
            var currentPolicy = await context.InsurancePolicies
                .FirstOrDefaultAsync(p => p.VehicleId == request.VehicleId && p.IsActive, cancellationToken);

            if (currentPolicy != null)
            {

                currentPolicy.IsActive = false;
                currentPolicy.EndDate = DateTime.UtcNow;
            }

            var insurancePolicy = new InsurancePolicy
            {
                CompanyName = request.CompanyName,
                AnnualPrice = request.AnnualPrice,
                AnnualMileageLimit = request.AnnualMileageLimit,
                VehicleId = request.VehicleId,
                RenewalDate = request.RenewalDate,
                StartingMileage = request.StartingMileage,
                IsActive = true,
                EndDate = request.RenewalDate.AddYears(1)
            };

            context.InsurancePolicies.Add(insurancePolicy);
            await context.SaveChangesAsync(cancellationToken);
            return new Response(insurancePolicy.Id, insurancePolicy.CompanyName,
                insurancePolicy.AnnualPrice, insurancePolicy.AnnualMileageLimit, insurancePolicy.VehicleId,
                insurancePolicy.RenewalDate, insurancePolicy.StartingMileage, insurancePolicy.IsActive, insurancePolicy.EndDate);
        }
    }

    public class EndPoint : IEndpointDefinition
    {
        public void MapEndpoints(WebApplication app)
        {
            app.MapPost("api/insurance", async (ISender sender, CreateInsurance.Command command, CancellationToken cancellationToken) =>
            {
                var result = await sender.Send(command, cancellationToken);
                return Results.Ok(result.Value);
            });
        }
    }
}

//[ApiController]
//[Route("api/insurance")]
//public class CreateInsuranceController(ISender sender) : ControllerBase
//{
//    private readonly ISender sender = sender;

//    [HttpPost]
//    public async Task<ActionResult<CreateInsurance.Response>> CreateInsurance([FromBody] CreateInsurance.Command command)
//    {
//        var result = await sender.Send(command);
//        return Ok(result.Value);
//    }
//}
