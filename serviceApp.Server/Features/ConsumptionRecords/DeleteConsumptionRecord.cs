namespace serviceApp.Server.Features.ConsumptionRecords;

public static class DeleteConsumptionRecord
{
    public record Command(int Id) : ICommand<bool>;

    public class Handler(ApplicationDbContext context) : ICommandHandler<Command, bool>
    {
        private readonly ApplicationDbContext context = context;
        public async Task<Result<bool>> Handle(Command request, CancellationToken cancellationToken)
        {
            var consumptionRecord = await context.ConsumptionRecords
                .Include(c => c.MileageHistory)
                .FirstOrDefaultAsync(c => c.Id == request.Id, cancellationToken);

            if (consumptionRecord == null)
            {
                return Result.Fail<bool>($"ConsumptionRecord with ID {request.Id} not found.");
            }

            if (consumptionRecord.MileageHistory != null)
            {
                context.MileageHistories.Remove(consumptionRecord.MileageHistory);
            }

            context.ConsumptionRecords.Remove(consumptionRecord);
            await context.SaveChangesAsync(cancellationToken);
            return true;
        }
    }

    public class EndPoint : IEndpointDefinition
    {
        public void MapEndpoints(WebApplication app)
        {
            app.MapDelete("api/consumption-record/{id}", async (ISender sender, int id, CancellationToken cancellationToken) =>
            {
                var result = await sender.Send(new Command(id), cancellationToken);
                if (result.Failure)
                {
                    return Results.NotFound(result.Error);
                }
                return Results.Ok(true);
            }).RequireAuthorization(); ;
        }
    }
}


