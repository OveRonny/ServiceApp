namespace serviceApp.Server.Features.ConsumptionRecords;

public static class CreateConsumptionRecord
{
    public record Command(int VehicleId, decimal DieselAdded, decimal DieselPricePerLiter, int Mileage, int? Hours) : ICommand<Response>;
    public record Response(int Id, int VehicleId, DateTime Date, decimal DieselAdded, decimal DieselPricePerLiter,
        decimal TotalCost, decimal? DieselConsumption, int Mileage);

    public class Handler(ApplicationDbContext context) : ICommandHandler<Command, Response>
    {
        private readonly ApplicationDbContext context = context;
        public async Task<Result<Response>> Handle(Command request, CancellationToken cancellationToken)
        {
            var mileage = await CreateMileageAsync(request, cancellationToken);

            var consumptionRecord = new ConsumptionRecord
            {
                VehicleId = request.VehicleId,
                DieselAdded = request.DieselAdded,
                DieselPricePerLiter = request.DieselPricePerLiter,
                Date = DateTime.Now,
                MileageHistoryId = mileage.Id,
            };
            context.ConsumptionRecords.Add(consumptionRecord);
            await context.SaveChangesAsync(cancellationToken);

            await LoadRelatedDataAsync(consumptionRecord, cancellationToken);

            return new Response(consumptionRecord.Id, consumptionRecord.VehicleId, consumptionRecord.Date, consumptionRecord.DieselAdded, consumptionRecord.DieselPricePerLiter,
                consumptionRecord.TotalCost, consumptionRecord.DieselConsumption, mileage.Mileage);
        }

        private async Task<MileageHistory> CreateMileageAsync(Command request, CancellationToken cancellationToken)
        {
            var mileage = new MileageHistory
            {
                Mileage = request.Mileage,
                VehicleId = request.VehicleId,
                Hours = request.Hours,
                RecordedDate = DateTime.Now,
                Type = MileageHistory.MileageType.Forbruk
            };

            context.MileageHistories.Add(mileage);
            await context.SaveChangesAsync(cancellationToken);

            return mileage;
        }

        private async Task LoadRelatedDataAsync(ConsumptionRecord consumptionRecord, CancellationToken cancellationToken)
        {
            await context.Entry(consumptionRecord)
                .Reference(c => c.MileageHistory)
                .Query()
                .Include(m => m.Vehicle)
                .ThenInclude(v => v!.MileageHistories)
                .LoadAsync(cancellationToken);
        }
    }
}

[ApiController]
[Route("api/consumption-record")]
public class CreateConsumptionRecordController(ISender sender) : ControllerBase
{
    private readonly ISender sender = sender;

    [HttpPost]
    public async Task<ActionResult<CreateConsumptionRecord.Response>> CreateConsumptionRecord([FromBody] CreateConsumptionRecord.Command command)
    {
        var result = await sender.Send(command);
        return Ok(result.Value);
    }
}
