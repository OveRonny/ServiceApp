namespace serviceApp.Server.Features.ServiceRecords;

public static class DeleteServiceRecord
{
    public record Command(int Id) : ICommand<bool>;
    public class Handler(ApplicationDbContext context) : ICommandHandler<Command, bool>
    {
        private readonly ApplicationDbContext context = context;
        public async Task<Result<bool>> Handle(Command request, CancellationToken cancellationToken)
        {
            var serviceRecord = await context.ServiceRecords
                .Include(s => s.MileageHistory)
                .Include(s => s.Parts)
                .FirstOrDefaultAsync(s => s.Id == request.Id, cancellationToken);

            if (serviceRecord == null)
            {
                return Result.Fail<bool>($"ServiceRecord with ID {request.Id} not found.");
            }

            if (serviceRecord.Parts != null && serviceRecord.Parts.Any())
            {
                foreach (var part in serviceRecord.Parts)
                {
                    // If the Parts entity stores VehicleInventoryId, try to find the inventory row
                    if (part.VehicleInventoryId != 0) // assuming 0 means unset; adjust if nullable
                    {
                        var inventory = await context.VehicleInventories
                            .FirstOrDefaultAsync(v => v.Id == part.VehicleInventoryId && v.VehicleId == serviceRecord.VehicleId, cancellationToken);

                        if (inventory != null)
                        {
                            inventory.QuantityInStock = (inventory.QuantityInStock ?? 0) + part.Quantity;
                            // tracked entity will be saved; no explicit Update required
                        }
                        // If inventory is missing, skip restoration to avoid throwing during delete.
                        // Optionally log or handle this case if you prefer stricter behavior.
                    }
                }

                context.Parts.RemoveRange(serviceRecord.Parts);
            }

            if (serviceRecord.MileageHistory != null)
            {
                context.MileageHistories.Remove(serviceRecord.MileageHistory);
            }

           

            context.ServiceRecords.Remove(serviceRecord);
            await context.SaveChangesAsync(cancellationToken);
            return true;
        }
    }

    public class EndPoint : IEndpointDefinition
    {
        public void MapEndpoints(WebApplication app)
        {
            app.MapDelete("api/service-record/{id}", async (ISender sender, int id, CancellationToken cancellationToken) =>
            {
                var result = await sender.Send(new Command(id), cancellationToken);
                if (result.Failure)
                {
                    return false;
                }
                return true;
            }).RequireAuthorization();
        }
    }
}


