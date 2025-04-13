namespace serviceApp.Server.Features.InsurancePolicies;

public static class DeleteInsurance
{
    public record Command(int Id) : ICommand<bool>;

    public class Handler(ApplicationDbContext context) : ICommandHandler<Command, bool>
    {
        private readonly ApplicationDbContext context = context;
        public async Task<Result<bool>> Handle(Command request, CancellationToken cancellationToken)
        {
            var insurancePolicy = await context.InsurancePolicies
                .FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken);
            if (insurancePolicy == null)
            {
                return Result.Fail<bool>($"Insurance policy with ID {request.Id} not found.");
            }
            // Mark the policy as inactive
            insurancePolicy.IsActive = false;
            insurancePolicy.EndDate = DateTime.UtcNow;

            context.InsurancePolicies.Remove(insurancePolicy);
            await context.SaveChangesAsync(cancellationToken);

            return Result.Ok(true);
        }
    }
}


[ApiController]
[Route("api/insurance")]
public class DeleteInsuranceController(ISender sender) : ControllerBase
{
    private readonly ISender sender = sender;

    [HttpDelete("{id}")]
    public async Task<ActionResult<bool>> DeleteInsurance(int id)
    {
        var result = await sender.Send(new DeleteInsurance.Command(id));
        if (result.Failure)
        {
            return NotFound(result.Error);
        }
        return Ok(result.Value);
    }
}
