namespace serviceApp.Server.Features.ConsumptionRecords;

public static class GetAllConsumptionRecord
{
    public record Query(int VehicleId, DateTime? StartDate, DateTime? EndDate) : IQuery<(List<Response> Records, ConsumptionSummaryResponse Summary)>;

    public record Response(int Id, int VehicleId, DateTime Date, decimal DieselAdded, decimal DieselPricePerLiter,
        decimal TotalCost, decimal? DieselConsumption, int MileageHistoryId, int Mileage, int? Hours, string Make, string Model, int? DrivenKm);


    public record ConsumptionSummaryResponse(
        decimal? AvgConsumption,
        decimal? AveragePrice,
        int TotalDriven,
        decimal? TotalCost,
        decimal? TotalDieselAdded
    );

    public class Handler : IQueryHandler<Query, (List<Response> Records, ConsumptionSummaryResponse Summary)>
    {
        private readonly ApplicationDbContext _context;

        public Handler(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<Result<(List<Response> Records, ConsumptionSummaryResponse Summary)>> Handle(Query request, CancellationToken cancellationToken)
        {
            var query = _context.ConsumptionRecords
                .Include(c => c.MileageHistory)
                .Include(v => v.Vehicle)
                .Where(c => c.VehicleId == request.VehicleId);

            DateTime? start = request.StartDate?.Date;
            DateTime? endExclusive = request.EndDate?.Date.AddDays(1);

            if (!start.HasValue && !endExclusive.HasValue)
            {
                var today = DateTime.Today;
                start = today;
                endExclusive = today.AddDays(1);
            }

            if (start.HasValue)
            {
                query = query.Where(c => c.Date >= start.Value);
            }

            if (endExclusive.HasValue)
            {
                query = query.Where(c => c.Date < endExclusive.Value);
            }

            var consumptionRecords = await query.ToListAsync(cancellationToken);

            var calculateDriven = new CalculateTotalDrivenService();

            int totalDriven = calculateDriven.CalculateTotalDriven(consumptionRecords, start, endExclusive);
            int drivenLastRecordToNow = calculateDriven.CalculateDrivenLastRecordToNow(consumptionRecords);

            var calculator = new FuelCalculatorService();

            decimal? avgConsumption = calculator.CalculateAveragePer100Km(consumptionRecords);
            decimal? avgPrice = calculator.CalculateAveragePrice(consumptionRecords, start, endExclusive);

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
                v.Vehicle.Model,
                v.DrivenKm
            )).ToList();

            var summary = new ConsumptionSummaryResponse(
                AvgConsumption: avgConsumption,
                AveragePrice: avgPrice,
                TotalDriven: totalDriven,
                TotalCost: consumptionRecords.Sum(c => c.TotalCost),
                TotalDieselAdded: consumptionRecords.Sum(c => c.DieselAdded)
            );

            return Result.Ok((response, summary));
        }
    }

    public class EndPoint : IEndpointDefinition
    {
        public void MapEndpoints(WebApplication app)
        {
            app.MapGet("api/consumption-record/vehicle/{vehicleId}", async (ISender sender, int vehicleId,
                DateTime? startDate, DateTime? endDate, CancellationToken cancellationToken) =>
            {
                var result = await sender.Send(new Query(vehicleId, startDate, endDate), cancellationToken);
                if (result.Failure)
                {
                    return Results.NotFound(result.Error);
                }
                return Results.Ok(new
                {
                    result.Value.Records,
                    result.Value.Summary
                });
            }).RequireAuthorization(); ;
        }
    }
}


