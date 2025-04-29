namespace serviceApp.Server.Features.ServiceRecords;

public static class GetAllServiceRecord
{
    public record Query(int VehicleId) : IQuery<List<Response>>;

    public record Response(int Id, int VehicleId, int ServiceTypeId, DateTime ServiceDate, string Description,
        decimal Cost, int MileageHistoryId, int Mileage, int? Hours, string VehicleName, string? ServiceTypeName);


    public class Handler(ApplicationDbContext context) : IQueryHandler<Query, List<Response>>
    {
        private readonly ApplicationDbContext context = context;
        public async Task<Result<List<Response>>> Handle(Query request, CancellationToken cancellationToken)
        {
            var serviceRecords = await context.ServiceRecords
                .Include(s => s.ServiceType)
                .Include(s => s.MileageHistory)
                .Include(c => c.Vehicle)
                .Where(s => s.VehicleId == request.VehicleId)
                .ToListAsync(cancellationToken);

            var serviceRecordDtos = serviceRecords.Select(v => new Response(
                v.Id,
                v.VehicleId,
                v.ServiceTypeId,
                v.ServiceDate,
                v.Description,
                v.Cost,
                v.MileageHistoryId,
                v.MileageHistory!.Mileage,
                v.MileageHistory.Hours,
                v.MileageHistory.Vehicle?.Make + " " + v.MileageHistory?.Vehicle?.Model,
                v.ServiceType?.Name

            )).ToList();

            return Result.Ok(serviceRecordDtos);
        }
    }

    public class EndPoint : IEndpointDefinition
    {
        public void MapEndpoints(WebApplication app)
        {
            app.MapGet("api/service-record/vehicle/{vehicleId}", async (ISender sender, int vehicleId, CancellationToken cancellationToken) =>
            {
                var result = await sender.Send(new Query(vehicleId), cancellationToken);
                return Results.Ok(result.Value);
            });
        }
    }
}


