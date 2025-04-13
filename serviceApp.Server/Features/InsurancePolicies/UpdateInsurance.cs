
namespace serviceApp.Server.Features.InsurancePolicies;

public static class UpdateInsurance
{
    public record Command(int Id, string CompanyName, decimal AnnualPrice, int AnnualMileageLimit, int VehicleId,
        DateTime RenewalDate, int StartingMileage) : ICommand<Response>;

    public record Response(int Id, string CompanyName, decimal AnnualPrice, int AnnualMileageLimit, int VehicleId,
        DateTime RenewalDate, int StartingMileage, bool IsActive, DateTime? EndDate);

    public class Handler(ApplicationDbContext context) : ICommandHandler<Command, Response>
    {
        private readonly ApplicationDbContext context = context;

        public async Task<Result<Response>> Handle(Command request, CancellationToken cancellationToken)
        {
            // Retrieve the current active policy for the vehicle
            var currentPolicy = await context.InsurancePolicies
                .FirstOrDefaultAsync(p => p.VehicleId == request.VehicleId && p.IsActive, cancellationToken);

            if (currentPolicy != null)
            {
                // Mark the current policy as inactive
                currentPolicy.IsActive = false;
                currentPolicy.EndDate = DateTime.UtcNow;
            }

            // Create a new policy with the corrected details
            var newPolicy = new InsurancePolicy
            {
                CompanyName = request.CompanyName,
                AnnualPrice = request.AnnualPrice,
                AnnualMileageLimit = request.AnnualMileageLimit,
                VehicleId = request.VehicleId,
                RenewalDate = request.RenewalDate,
                StartingMileage = request.StartingMileage,
                IsActive = true, // New policy is active
                EndDate = null // Active policy has no end date
            };

            // Add the new policy to the database
            context.InsurancePolicies.Add(newPolicy);

            // Save changes to the database
            await context.SaveChangesAsync(cancellationToken);

            // Return the new policy as a response
            return Result.Ok(new Response(
                newPolicy.Id,
                newPolicy.CompanyName,
                newPolicy.AnnualPrice,
                newPolicy.AnnualMileageLimit,
                newPolicy.VehicleId,
                newPolicy.RenewalDate,
                newPolicy.StartingMileage,
                newPolicy.IsActive,
                newPolicy.EndDate
            ));
        }
    }
}

[ApiController]
[Route("api/insurance")]
public class UpdateInsuranceController(ISender sender) : ControllerBase
{
    private readonly ISender sender = sender;

    [HttpPut("{id}")]
    public async Task<ActionResult<UpdateInsurance.Response>> UpdateInsurance(int id, [FromBody] UpdateInsurance.Command command)
    {
        if (id != command.Id)
        {
            return BadRequest("ID in the URL does not match the ID in the request body.");
        }
        var result = await sender.Send(command);
        if (result.Failure)
        {
            return NotFound(result.Error);
        }
        return Ok(result.Value);
    }
}
