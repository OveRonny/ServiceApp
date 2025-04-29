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

    public class EndPoint : IEndpointDefinition
    {
        public void MapEndpoints(WebApplication app)
        {
            app.MapPut("api/service-record/{id}", async (ISender sender, int id, UpdateServiceRecord.Command command, CancellationToken cancellationToken) =>
            {
                if (id != command.Id)
                {
                    return Results.BadRequest("Service record ID mismatch.");
                }
                var result = await sender.Send(command, cancellationToken);
                if (result.Failure)
                {
                    return Results.NotFound(result.Error);
                }
                return Results.Ok(result.Value);
            });
        }
    }
}

