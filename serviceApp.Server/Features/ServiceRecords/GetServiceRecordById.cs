namespace serviceApp.Server.Features.ServiceRecords;

public static class GetServiceRecordById
{
    public record Query(int Id) : IQuery<Response>;

    public record Response(int Id, int VehicleId, DateTime ServiceDate, decimal Cost, string Description,
        int MileageHistoryId, int Mileage, int? Hours, string VehicleName, List<Parts> Parts, int ServiceTypeId, int ServiceCompanyId);
    public class Handler(ApplicationDbContext context) : IQueryHandler<Query, Response>
    {
        private readonly ApplicationDbContext context = context;
        public async Task<Result<Response>> Handle(Query request, CancellationToken cancellationToken)
        {
            var serviceRecord = await context.ServiceRecords
                .Include(s => s.MileageHistory)
                .Include(s => s.Vehicle)
                .Include(v => v.ServiceCompany)
                .Include(h => h.Parts)
                .Include(d => d.ServiceType)
                .FirstOrDefaultAsync(s => s.Id == request.Id, cancellationToken);

            if (serviceRecord == null)
            {
                return Result.Fail<Response>($"ServiceRecord with ID {request.Id} not found.");
            }

            var response = new Response(
                serviceRecord.Id,
                serviceRecord.VehicleId,
                serviceRecord.ServiceDate,
                serviceRecord.Cost,
                serviceRecord.Description,
                serviceRecord.MileageHistoryId,
                serviceRecord.MileageHistory!.Mileage,
                serviceRecord.MileageHistory.Hours,
                serviceRecord.MileageHistory.Vehicle?.Make + " " + serviceRecord.MileageHistory?.Vehicle?.Model,
                serviceRecord.Parts?.Select(p => new Parts
                {
                    Id = p.Id,
                    Name = p.Name,
                    Price = p.Price,
                    VehicleInventoryId = p.VehicleInventoryId,
                    Description = p.Description,
                    Quantity = p.Quantity
                }).ToList() ?? new List<Parts>(),
                serviceRecord.ServiceTypeId,
                serviceRecord.ServiceCompanyId
            );
            return Result.Ok(response);
        }
    }

    public class EndPoint : IEndpointDefinition
    {
        public void MapEndpoints(WebApplication app)
        {
            app.MapGet("api/service-record/{id}", async (ISender sender, int id, CancellationToken cancellationToken) =>
            {
                var result = await sender.Send(new Query(id), cancellationToken);
                if (result.Failure)
                {
                    return Results.NotFound(result.Error);
                }
                return Results.Ok(result.Value);
            });
        }
    }
}


