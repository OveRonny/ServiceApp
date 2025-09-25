namespace serviceApp.Server.Features.InsurancePolicies;

public static class GetInsuranceById
{
    public record Query(int Id) : IQuery<Response>;
    public record Response(InsurancePolicyDto InsurancePolicy);
    public record InsurancePolicyDto(int Id, string CompanyName, decimal AnnualPrice, int AnnualMileageLimit,
        int VehicleId, DateTime RenewalDate, int StartingMileage, bool IsActive, DateTime? EndDate);
    public class Handler(ApplicationDbContext context) : IQueryHandler<Query, Response>
    {
        private readonly ApplicationDbContext context = context;
        public async Task<Result<Response>> Handle(Query request, CancellationToken cancellationToken)
        {
            var insurancePolicy = await context.InsurancePolicies
                .Include(i => i.Vehicle)
                .FirstOrDefaultAsync(i => i.Id == request.Id, cancellationToken);
            if (insurancePolicy == null)
            {
                return Result.Fail<Response>($"Insurance policy with ID {request.Id} not found.");
            }
            var insurancePolicyDto = new InsurancePolicyDto(
                insurancePolicy.Id,
                insurancePolicy.CompanyName,
                insurancePolicy.AnnualPrice,
                insurancePolicy.AnnualMileageLimit,
                insurancePolicy.VehicleId,
                insurancePolicy.RenewalDate,
                insurancePolicy.StartingMileage,
                insurancePolicy.IsActive,
                insurancePolicy.EndDate
            );
            return Result.Ok(new Response(insurancePolicyDto));
        }
    }

    public class EndPoint : IEndpointDefinition
    {
        public void MapEndpoints(WebApplication app)
        {
            app.MapGet("api/insurance/{id}", async (ISender sender, int id, CancellationToken cancellationToken) =>
            {
                var result = await sender.Send(new Query(id), cancellationToken);
                if (result.Failure)
                {
                    return Results.NotFound(result.Error);
                }
                return Results.Ok(result.Value);
            }).RequireAuthorization();
        }
    }
}

