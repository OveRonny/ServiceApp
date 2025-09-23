namespace serviceApp.Server.Features.ConsumptionRecords;

public static class GetConsumptionRecordById
{
    public record Query(int Id) : IQuery<Response>;
    public record Response(int Id, int VehicleId, DateTime Date, decimal DieselAdded, decimal DieselPricePerLiter,
        decimal TotalCost, decimal? DieselConsumption, int MileageHistoryId, int Mileage, int? Hours);
    public class Handler(ApplicationDbContext context) : IQueryHandler<Query, Response>
    {
        private readonly ApplicationDbContext context = context;
        public async Task<Result<Response>> Handle(Query request, CancellationToken cancellationToken)
        {
            var consumptionRecord = await context.ConsumptionRecords
                .Include(c => c.MileageHistory)
                .ThenInclude(m => m!.Vehicle)
                .ThenInclude(v => v!.MileageHistories)
                .FirstOrDefaultAsync(c => c.Id == request.Id, cancellationToken);

            if (consumptionRecord == null)
            {
                return Result.Fail<Response>($"ConsumptionRecord with ID {request.Id} not found.");
            }
            var response = new Response(
                consumptionRecord.Id,
                consumptionRecord.VehicleId,
                consumptionRecord.Date,
                consumptionRecord.DieselAdded,
                consumptionRecord.DieselPricePerLiter,
                consumptionRecord.TotalCost,
                consumptionRecord.DieselConsumption,
                consumptionRecord.MileageHistoryId,
                consumptionRecord.MileageHistory!.Mileage,
                consumptionRecord.MileageHistory.Hours
            );
            return Result.Ok(response);
        }
    }

    public class EndPoint : IEndpointDefinition
    {
        public void MapEndpoints(WebApplication app)
        {
            app.MapGet("api/consumption-record/{id}", async (ISender sender, int id, CancellationToken cancellationToken) =>
            {
                var result = await sender.Send(new Query(id), cancellationToken);
                if (result.Failure)
                {
                    return Results.NotFound(result.Error);
                }
                return Results.Ok(result.Value);
            }).RequireAuthorization(); ;
        }
    }
}

