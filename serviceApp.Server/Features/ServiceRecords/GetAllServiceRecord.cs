namespace serviceApp.Server.Features.ServiceRecords;

public static class GetAllServiceRecord
{
    public record Query(int VehicleId) : IQuery<Response>;

    public record Response(List<ServiceRecordDto> ServiceRecords);
    public record ServiceRecordDto(int Id, int VehicleId, int ServiceTypeId, DateTime ServiceDate, string Description,
        decimal Cost, int MileageHistoryId, int Mileage, int? Hours, string VehicleName);

    public class Handler(ApplicationDbContext context) : IQueryHandler<Query, Response>
    {
        private readonly ApplicationDbContext context = context;
        public async Task<Result<Response>> Handle(Query request, CancellationToken cancellationToken)
        {
            var serviceRecords = await context.ServiceRecords
                .Include(s => s.MileageHistory)
                .Include(c => c.Vehicle)
                .Where(s => s.VehicleId == request.VehicleId)
                .ToListAsync(cancellationToken);

            var serviceRecordDtos = serviceRecords.Select(v => new ServiceRecordDto(
                v.Id,
                v.VehicleId,
                v.ServiceTypeId,
                v.ServiceDate,
                v.Description,
                v.Cost,
                v.MileageHistoryId,
                v.MileageHistory!.Mileage,
                v.MileageHistory.Hours,
                v.MileageHistory.Vehicle?.Make + " " + v.MileageHistory?.Vehicle?.Model

            )).ToList();

            var response = new Response(serviceRecordDtos);
            return Result.Ok(response);
        }
    }
}

[ApiController]
[Route("api/service-record")]
public class GetAllServiceRecordController(ISender sender) : ControllerBase
{
    private readonly ISender sender = sender;

    [HttpGet("vehicle/{vehicleId}")]
    public async Task<ActionResult<GetAllServiceRecord.Response>> GetAllServiceRecord(int vehicleId)
    {
        var result = await sender.Send(new GetAllServiceRecord.Query(vehicleId));
        return Ok(result);
    }
}
