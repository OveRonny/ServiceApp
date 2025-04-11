namespace serviceApp.Server.Features.ConsumptionRecords;

public static class UpdateConsumptionRecord
{
    public record Command(int Id, int VehicleId, decimal DieselAdded, decimal DieselPricePerLiter, int Mileage, int? Hours) : ICommand<Response>;
    public record Response(int Id, int VehicleId, DateTime Date, decimal DieselAdded, decimal DieselPricePerLiter,
        decimal TotalCost, decimal? DieselConsumption, int Mileage);

    public class Handler(ApplicationDbContext context) : ICommandHandler<Command, Response>
    {
        private readonly ApplicationDbContext context = context;

        public async Task<Result<Response>> Handle(Command request, CancellationToken cancellationToken)
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

            consumptionRecord.VehicleId = request.VehicleId;
            consumptionRecord.DieselAdded = request.DieselAdded;
            consumptionRecord.DieselPricePerLiter = request.DieselPricePerLiter;


            var updatedMileage = await UpdateMilage(consumptionRecord.MileageHistoryId, request, cancellationToken);
            consumptionRecord.MileageHistoryId = updatedMileage.Id;


            context.ConsumptionRecords.Update(consumptionRecord);
            await context.SaveChangesAsync(cancellationToken);


            return Result.Ok(new Response(
                consumptionRecord.Id,
                consumptionRecord.VehicleId,
                consumptionRecord.Date,
                consumptionRecord.DieselAdded,
                consumptionRecord.DieselPricePerLiter,
                consumptionRecord.TotalCost,
                consumptionRecord.DieselConsumption,
                updatedMileage.Mileage
            ));

        }


        private async Task<MileageHistory> UpdateMilage(int mileageHistoryId, Command request, CancellationToken cancellationToken)
        {
            var mileage = await context.MileageHistories
                .FirstOrDefaultAsync(m => m.Id == mileageHistoryId, cancellationToken);

            if (mileage == null)
            {
                throw new InvalidOperationException($"MileageHistory with ID {mileageHistoryId} not found.");
            }


            mileage.Mileage = request.Mileage;
            mileage.Hours = request.Hours;
            mileage.RecordedDate = DateTime.Now;

            context.MileageHistories.Update(mileage);
            await context.SaveChangesAsync(cancellationToken);

            return mileage;
        }
    }
}

[ApiController]
[Route("api/consumption-record")]
public class UpdateConsumptionRecordController(ISender sender) : ControllerBase
{
    private readonly ISender sender = sender;

    [HttpPut("{id}")]
    public async Task<ActionResult<UpdateConsumptionRecord.Response>> UpdateConsumptionRecord(int id, [FromBody] UpdateConsumptionRecord.Command command)
    {
        if (id != command.Id)
        {
            return BadRequest("ConsumptionRecord ID mismatch.");
        }
        var result = await sender.Send(command);
        if (result.Failure)
        {
            return NotFound(result.Error);
        }
        return Ok(result.Value);
    }
}
