using serviceApp.Server.Features.Autentication;
using static serviceApp.Server.Features.ServiceRecords.CreateServiceRecord;

namespace serviceApp.Server.Features.ServiceRecords;

public static class UpdateServiceRecord
{
    public record Command(int Id, int VehicleId, int ServiceTypeId, string Description, decimal? Cost, int Mileage, int? Hours, int ServiceCompanyId, IReadOnlyList<UsedPart> UsedParts) : ICommand<Response>;

    public record Response(int Id, int VehicleId, int ServiceTypeId, DateTime ServiceDate, string Description, decimal? Cost, int Mileage, int? Hours);

    public class Handler(ApplicationDbContext context, ICurrentUser user) : ICommandHandler<Command, Response>
    {
        private readonly ApplicationDbContext context = context;
        private readonly ICurrentUser currentUser = user;

        public async Task<Result<Response>> Handle(Command request, CancellationToken cancellationToken)
        {
            var familyId = await currentUser.GetFamilyIdAsync(cancellationToken);
            if (familyId is null)
                return Result.Fail<Response>("Not authenticated.");

            var serviceRecord = await context.ServiceRecords
                .Include(s => s.MileageHistory)
                .Include(r => r.Parts)
                .FirstOrDefaultAsync(i => i.Id == request.Id, cancellationToken);

            if (serviceRecord == null)
            {
                return Result.Fail<Response>($"Service record with ID {request.VehicleId} not found.");
            }

            var existingParts = serviceRecord.Parts.Where(p => p.VehicleInventoryId.HasValue).ToList();
            if (existingParts.Count > 0)
            {
                var invIds = existingParts.Select(p => p.VehicleInventoryId!.Value).Distinct().ToList();
                var invs = await context.VehicleInventories.Where(i => invIds.Contains(i.Id)).ToListAsync(cancellationToken);
                foreach (var p in existingParts)
                {
                    var inv = invs.First(i => i.Id == p.VehicleInventoryId);
                    inv.QuantityInStock = (inv.QuantityInStock ?? 0) + p.Quantity; // add back
                }
            }

            serviceRecord.Parts.Clear();

            // Apply new used parts
            var grouped = (request.UsedParts ?? Array.Empty<UsedPart>())
                .Where(p => p.Quantity > 0)
                .GroupBy(p => p.VehicleInventoryId)
                .Select(g => new { VehicleInventoryId = g.Key, Quantity = g.Sum(x => x.Quantity) })
                .ToList();

            if (grouped.Count > 0)
            {
                var invIds = grouped.Select(g => g.VehicleInventoryId).ToList();
                var inventories = await context.VehicleInventories
                    .Where(i => invIds.Contains(i.Id) && i.VehicleId == request.VehicleId)
                    .ToListAsync(cancellationToken);

                if (inventories.Count != grouped.Count)
                    return Result.Fail<Response>("One or more inventory items were not found for this vehicle.");

                foreach (var g in grouped)
                {
                    var inv = inventories.First(i => i.Id == g.VehicleInventoryId);
                    var stock = inv.QuantityInStock ?? 0;
                    if (stock < g.Quantity)
                        return Result.Fail<Response>($"Not enough stock for '{inv.PartName}'. Available: {stock}");

                    inv.QuantityInStock = stock - g.Quantity;

                    serviceRecord.Parts.Add(new Parts
                    {
                        Name = string.IsNullOrWhiteSpace(inv.PartName) ? "Part" : inv.PartName,
                        Price = inv.Cost,
                        Description = inv.Description ?? string.Empty,
                        Quantity = g.Quantity,
                        VehicleInventoryId = inv.Id
                    });
                }
            }


            var mileage = await UpdateMileageAsync(serviceRecord.MileageHistoryId, request, cancellationToken);

            serviceRecord.VehicleId = request.VehicleId;
            serviceRecord.ServiceTypeId = request.ServiceTypeId;
            serviceRecord.Description = request.Description;
            serviceRecord.Cost = request.Cost;
            serviceRecord.ServiceCompanyId = request.ServiceCompanyId;
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
            }).RequireAuthorization(); ;
        }
    }
}

