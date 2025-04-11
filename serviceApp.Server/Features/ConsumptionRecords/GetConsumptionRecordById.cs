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
}

[ApiController]
[Route("api/consumption-record")]
public class GetConsumptionRecordByIdController(ISender sender) : ControllerBase
{
    private readonly ISender sender = sender;

    [HttpGet("{id}")]

    public async Task<ActionResult<GetConsumptionRecordById.Response>> GetConsumptionRecordById(int id)
    {
        var result = await sender.Send(new GetConsumptionRecordById.Query(id));
        if (result.Failure)
        {
            return NotFound(result.Error);
        }
        return Ok(result.Value);
    }

}