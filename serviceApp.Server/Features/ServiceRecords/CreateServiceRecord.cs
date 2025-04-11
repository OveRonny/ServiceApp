namespace serviceApp.Server.Features.ServiceRecords;

public static class CreateServiceRecord
{
    public record Command(int VehicleId, int ServiceTypeId, string Description, decimal Cost, int Mileage, int? Hours, int ServiceCompanyId) : ICommand<Response>;

    public record Response(int Id, int VehicleId, int ServiceTypeId, DateTime ServiceDate, string Description, decimal Cost, int Mileage, int? Hours);

    public class Handler(ApplicationDbContext context) : ICommandHandler<Command, Response>
    {
        private readonly ApplicationDbContext context = context;
        public async Task<Result<Response>> Handle(Command request, CancellationToken cancellationToken)
        {
            var mileage = await CreateMileageAsync(request, cancellationToken);

            var serviceRecord = new ServiceRecord
            {
                VehicleId = request.VehicleId,
                ServiceTypeId = request.ServiceTypeId,
                ServiceDate = DateTime.Now,
                Description = request.Description,
                Cost = request.Cost,
                MileageHistoryId = mileage.Id,
                ServiceCompanyId = request.ServiceCompanyId

            };
            context.ServiceRecords.Add(serviceRecord);
            await context.SaveChangesAsync(cancellationToken);
            return new Response(serviceRecord.Id, serviceRecord.VehicleId,
                serviceRecord.ServiceTypeId, serviceRecord.ServiceDate, serviceRecord.Description,
                serviceRecord.Cost, mileage.Mileage, mileage.Hours);
        }

        private async Task<MileageHistory> CreateMileageAsync(Command request, CancellationToken cancellationToken)
        {
            var mileage = new MileageHistory
            {
                Mileage = request.Mileage,
                VehicleId = request.VehicleId,
                Hours = request.Hours,
                RecordedDate = DateTime.Now,
                Type = MileageHistory.MileageType.Service
            };
            context.MileageHistories.Add(mileage);
            await context.SaveChangesAsync(cancellationToken);
            return mileage;
        }
    }
}

[ApiController]
[Route("api/service-record")]
public class CreateServiceRecordController(ISender sender) : ControllerBase
{
    private readonly ISender sender = sender;

    [HttpPost]
    public async Task<ActionResult<CreateServiceRecord.Response>> CreateServiceRecord([FromBody] CreateServiceRecord.Command command)
    {
        var result = await sender.Send(command);
        return Ok(result.Value);
    }
}
