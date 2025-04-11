namespace serviceApp.Server.Features.ConsumptionRecords;

public static class GetAllConsumptionRecord
{
    public record Query(int VehicleId) : IQuery<Response>;
    public record Response(List<ConsumptionRecordDto> ConsumptionRecords);

    public record ConsumptionRecordDto(int Id, int VehicleId, DateTime Date, decimal DieselAdded, decimal DieselPricePerLiter,
        decimal TotalCost, decimal? DieselConsumption, int MileageHistoryId, int Mileage, int? Hours);

    public class Handler(ApplicationDbContext context) : IQueryHandler<Query, Response>
    {
        private readonly ApplicationDbContext context = context;
        public async Task<Result<Response>> Handle(Query request, CancellationToken cancellationToken)
        {
            var consumptionRecords = await context.ConsumptionRecords
                .Include(c => c.MileageHistory)
                .ThenInclude(m => m!.Vehicle)
                .ThenInclude(v => v!.MileageHistories)
                .Where(c => c.VehicleId == request.VehicleId)
                .ToListAsync(cancellationToken);

            var consumptionRecordDtos = consumptionRecords.Select(v => new ConsumptionRecordDto(
                v.Id,
                v.VehicleId,
                v.Date,
                v.DieselAdded,
                v.DieselPricePerLiter,
                v.TotalCost,
                v.DieselConsumption,
                v.MileageHistoryId,
                v.MileageHistory!.Mileage,
                v.MileageHistory.Hours
            )).ToList();

            var response = new Response(consumptionRecordDtos);

            return Result.Ok(response);
        }
    }
}

[ApiController]
[Route("api/consumption-record")]
public class GetAllConsumptionRecordController(ISender sender) : ControllerBase
{
    private readonly ISender sender = sender;

    [HttpGet("vehicle/{vehicleId}")]
    public async Task<ActionResult<GetAllConsumptionRecord.Response>> GetAllConsumptionRecord(int vehicleId)
    {
        var result = await sender.Send(new GetAllConsumptionRecord.Query(vehicleId));
        return Ok(result);
    }
}
