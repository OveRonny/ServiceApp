
namespace serviceApp.Server.Features.InsurancePolicies;

public static class UpdateInsurance
{
    public record Command(int Id, string CompanyName, decimal AnnualPrice, decimal TraficInsurancePrice, int AnnualMileageLimit, int VehicleId,
        DateTime RenewalDate, int StartingMileage) : ICommand<Response>;

    public record Response(int Id, string CompanyName, decimal AnnualPrice, decimal TraficInsurancePrice, int AnnualMileageLimit, int VehicleId,
        DateTime RenewalDate, int StartingMileage, bool IsActive, DateTime? EndDate);

    public class Handler(ApplicationDbContext context) : ICommandHandler<Command, Response>
    {
        private readonly ApplicationDbContext context = context;

        public async Task<Result<Response>> Handle(Command request, CancellationToken cancellationToken)
        {
            var policy = await context.InsurancePolicies
             .FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken);

            if (policy is null)
                return Result.Fail<Response>($"Insurance policy {request.Id} not found.");

            if (policy.VehicleId != request.VehicleId)
                return Result.Fail<Response>("VehicleId mismatch – changing vehicle is not allowed.");

            // (Optional) Prevent editing inactive (historical) policies
            if (!policy.IsActive)
                return Result.Fail<Response>("Cannot modify an inactive (archived) policy.");

            // If you consider RenewalDate change => treat as replacement, you could branch here.
            // For now we just update in place.
            policy.CompanyName        = request.CompanyName;
            policy.AnnualPrice        = request.AnnualPrice;
            policy.TraficInsurancePrice = request.TraficInsurancePrice;
            policy.AnnualMileageLimit = request.AnnualMileageLimit;
            policy.RenewalDate        = request.RenewalDate;
            policy.StartingMileage    = request.StartingMileage;

            // Keep active; do NOT touch EndDate unless logic dictates
            // If you want to auto-extend a 1‑year span, uncomment:
            // policy.EndDate = null; // or policy.RenewalDate.AddYears(1);

            await context.SaveChangesAsync(cancellationToken);

            // Return the new policy as a response
            return Result.Ok(new Response(
                policy.Id,
                policy.CompanyName,
                policy.AnnualPrice,
                policy.TraficInsurancePrice,
                policy.AnnualMileageLimit,
                policy.VehicleId,
                policy.RenewalDate,
                policy.StartingMileage,
                policy.IsActive,
                policy.EndDate
            ));
        }
    }

    public class EndPoint : IEndpointDefinition
    {
        public void MapEndpoints(WebApplication app)
        {
            app.MapPut("api/insurance/{id}", async (ISender sender, int id, UpdateInsurance.Command command, CancellationToken cancellationToken) =>
            {
                if (id != command.Id)
                {
                    return Results.BadRequest("ID in the URL does not match the ID in the request body.");
                }
                var result = await sender.Send(command, cancellationToken);
                if (result.Failure)
                {
                    return Results.NotFound(result.Error);
                }
                return Results.Ok(result.Value);
            }).RequireAuthorization();
        }
    }
}


