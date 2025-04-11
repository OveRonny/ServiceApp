namespace serviceApp.Server.Features.ServiceRecords;

public static class UpdateServiceRecord
{
    public record Command(int Id, int VehicleId, int ServiceTypeId, string Description, decimal Cost, int Mileage, int? Hours, int ServiceCompanyId) : ICommand<Response>;

    public record Response(int Id, int VehicleId, int ServiceTypeId, DateTime ServiceDate, string Description, decimal Cost, int Mileage, int? Hours);

    public class Handler(ApplicationDbContext context) : ICommandHandler<Command, Response>
    {
        private readonly ApplicationDbContext context = context;
        public async Task<Result<Response>> Handle(Command request, CancellationToken cancellationToken)
        {
            var serviceRecord = await context.ServiceRecords
                .Include(s => s.MileageHistory)
                .FirstOrDefaultAsync(i => i.Id == request.Id, cancellationToken);

            if (serviceRecord == null)
            {
                return Result.Fail<Response>($"Service record with ID {request.VehicleId} not found.");
            }

            var mileage = await UpdateMileageAsync(serviceRecord.MileageHistoryId, request, cancellationToken);

            serviceRecord.ServiceTypeId = request.ServiceTypeId;
            serviceRecord.Description = request.Description;
            serviceRecord.Cost = request.Cost;
            serviceRecord.MileageHistoryId = mileage.Id;

            await context.SaveChangesAsync(cancellationToken);
            return new Response(serviceRecord.Id, serviceRecord.VehicleId,
                serviceRecord.ServiceTypeId, serviceRecord.ServiceDate, serviceRecord.Description,
                serviceRecord.Cost, mileage.Mileage, mileage.Hours);
        }
        private async Task<MileageHistory> UpdateMileageAsync(int mileageHistoryId, Command request, CancellationToken cancellationToken)
        {
            var mileage = await context.MileageHistories
                .FirstOrDefaultAsync(m => m.Id == mileageHistoryId, cancellationToken);

            if (mileage == null)
            {
                throw new InvalidOperationException($"MileageHistory {request.Id} not found.");
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
[Route("api/service-record")]
public class UpdateServiceRecordController(ISender sender) : ControllerBase
{
    private readonly ISender sender = sender;
    [HttpPut("{id}")]
    public async Task<ActionResult<UpdateServiceRecord.Response>> UpdateServiceRecord(int id, [FromBody] UpdateServiceRecord.Command command)
    {
        if (id != command.Id)
        {
            return BadRequest("Service record ID mismatch.");
        }
        var result = await sender.Send(command);
        if (result.Failure)
        {
            return NotFound(result.Error);
        }
        return Ok(result.Value);
    }
}
