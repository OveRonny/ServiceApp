using System.Linq;
using serviceApp.Server.Features.Autentication;


namespace serviceApp.Server.Features.ServiceRecords;

public static class CreateServiceRecord
{
    public record UsedPart(int VehicleInventoryId, int Quantity);
    public record Command(int VehicleId, int ServiceTypeId, string Description, decimal? Cost, int Mileage,
        int? Hours, int ServiceCompanyId, DateTime ServiceDate, IReadOnlyList<UsedPart>? UsedParts = null) : ICommand<Response>;

    public record Response(int Id, int VehicleId, int ServiceTypeId, DateTime ServiceDate, string Description, decimal? Cost, int Mileage, int? Hours);

    public class Handler(ApplicationDbContext context, ICurrentUser currentUser) : ICommandHandler<Command, Response>
    {
        private readonly ApplicationDbContext context = context;
        private readonly ICurrentUser currentUser = currentUser;

        public async Task<Result<Response>> Handle(Command request, CancellationToken cancellationToken)
        {
            var familyId = await currentUser.GetFamilyIdAsync(cancellationToken);
            if (familyId is null)
                return Result.Fail<Response>("Not authenticated.");



            var mileage = await CreateMileageAsync(request, cancellationToken);

            var serviceRecord = new ServiceRecord
            {
                VehicleId = request.VehicleId,
                ServiceTypeId = request.ServiceTypeId,
                ServiceDate = request.ServiceDate,
                Description = request.Description,
                Cost = request.Cost,
                MileageHistoryId = mileage.Id,
                ServiceCompanyId = request.ServiceCompanyId
            };

            // Handle used parts (optional)
            var usedParts = (request.UsedParts ?? Array.Empty<UsedPart>())
                .Where(p => p.Quantity > 0)
                .ToList();

            if (usedParts.Count > 0)
            {
                // Group duplicates and sum quantities
                var grouped = usedParts
                    .GroupBy(p => p.VehicleInventoryId)
                    .Select(g => new { VehicleInventoryId = g.Key, Quantity = g.Sum(x => x.Quantity) })
                    .ToList();

                var invIds = grouped.Select(g => g.VehicleInventoryId).ToList();

                // Load inventory items for this vehicle (query filters should apply FamilyId)
                var inventories = await context.Set<VehicleInventory>()
                    .Where(i => invIds.Contains(i.Id) && i.VehicleId == request.VehicleId)
                    .ToListAsync(cancellationToken);

                if (inventories.Count != grouped.Count)
                {
                    return Result.Fail<Response>("One or more inventory items were not found for this vehicle.");
                }

                // Validate and decrement stock; create Parts snapshots
                foreach (var g in grouped)
                {
                    var inv = inventories.First(i => i.Id == g.VehicleInventoryId);
                    var currentStock = inv.QuantityInStock ?? 0;

                    if (currentStock < g.Quantity)
                    {
                        return Result.Fail<Response>($"Not enough stock for '{inv.PartName}'. Available: {currentStock}");
                    }

                    inv.QuantityInStock = currentStock - g.Quantity;

                    serviceRecord.Parts.Add(new Parts
                    {
                        Name = string.IsNullOrWhiteSpace(inv.PartName) ? "Part" : inv.PartName,
                        Price = inv.Cost,
                        Description = inv.Description ?? string.Empty,
                        Quantity = g.Quantity,
                        VehicleInventoryId = inv.Id,
                    });
                }

                // Calculate total parts cost and add to record cost
                var partsTotal = serviceRecord.Parts?.Sum(p => (p.Price ?? 0m) * p.Quantity) ?? 0m;
                serviceRecord.Cost = (request.Cost ?? 0m) + partsTotal;
               
            }

            context.ServiceRecords.Add(serviceRecord);
            await context.SaveChangesAsync(cancellationToken);

            return new Response(
                serviceRecord.Id,
                serviceRecord.VehicleId,
                serviceRecord.ServiceTypeId,
                serviceRecord.ServiceDate,
                serviceRecord.Description,
                serviceRecord.Cost,
                mileage.Mileage,
                mileage.Hours
            );
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

    public class EndPoint : IEndpointDefinition
    {
        public void MapEndpoints(WebApplication app)
        {
            app.MapPost("api/service-record", async (ISender sender, CreateServiceRecord.Command command, CancellationToken cancellationToken) =>
            {
                var result = await sender.Send(command, cancellationToken);
                return Results.Ok(result.Value);
            }).RequireAuthorization(); ;
        }
    }
}


