namespace serviceApp.Server.Features.ConsumptionRecords;

public static class GetAllConsumptionRecord
{
    public record Query(int VehicleId, DateTime? StartDate, DateTime? EndDate) : IQuery<List<Response>>;


    public record Response(int Id, int VehicleId, DateTime Date, decimal DieselAdded, decimal DieselPricePerLiter,
        decimal TotalCost, decimal? DieselConsumption, int MileageHistoryId, int Mileage, int? Hours, string Make, string Model);

    public record VehicleDto(int Id, string Make, string Model);


    public class Handler(ApplicationDbContext context) : IQueryHandler<Query, List<Response>>
    {
        private readonly ApplicationDbContext context = context;
        public async Task<Result<List<Response>>> Handle(Query request, CancellationToken cancellationToken)
        {
            var query = context.ConsumptionRecords
                .Include(c => c.MileageHistory)
                .Include(v => v.Vehicle)
                .Where(c => c.VehicleId == request.VehicleId);

            // Filtrer basert på tidsperiode
            if (request.StartDate.HasValue)
            {
                var startDate = request.StartDate.Value;
                query = query.Where(c => c.Date >= startDate); // Records on or after StartDate
            }

            if (request.EndDate.HasValue)
            {
                var endDate = request.EndDate.Value;
                query = query.Where(c => c.Date <= endDate); // Records on or before EndDate
            }

            var consumptionRecords = await query.ToListAsync(cancellationToken);

            var response = consumptionRecords.Select(v => new Response(
                v.Id,
                v.VehicleId,
                v.Date,
                v.DieselAdded,
                v.DieselPricePerLiter,
                v.TotalCost,
                v.DieselConsumption,
                v.MileageHistoryId,
                v.MileageHistory!.Mileage,
                v.MileageHistory.Hours,
                v.Vehicle!.Make,
                v.Vehicle.Model
            )).ToList();

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
    public async Task<ActionResult<List<GetAllConsumptionRecord.Response>>> GetAllConsumptionRecord(
        int vehicleId, [FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate)
    {
        // Sett standardverdier for siste år hvis ingen datoer er oppgitt
        startDate = startDate ?? DateTime.UtcNow.Date.AddYears(-1);
        endDate = endDate?.Date.AddDays(1).AddTicks(-1) ?? DateTime.UtcNow.Date.AddDays(1).AddTicks(-1);

        var result = await sender.Send(new GetAllConsumptionRecord.Query(vehicleId, startDate, endDate));
        return Ok(result.Value);
    }
}
