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

    public class EndPoint : IEndpointDefinition
    {
        public void MapEndpoints(WebApplication app)
        {
            app.MapDelete("api/insurance/{id}", async (ISender sender, int id, CancellationToken cancellationToken) =>
            {
                var result = await sender.Send(new Command(id), cancellationToken);
                if (result.Failure)
                {
                    return Results.NotFound(result.Error);
                }
                return Results.Ok(result.Value);
            }).RequireAuthorization();
        }
    }
}



