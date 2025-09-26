using serviceApp.Server.Features.Autentication;

namespace serviceApp.Server.Features.MilegaHistory;

public static class GetAllMileageHistory
{
    public record Query(int VehicleId) : IQuery<List<MileageHistory>>;
    public class Handler(ApplicationDbContext context, ICurrentUser currentUser) : IQueryHandler<Query, List<MileageHistory>>
    {
        private readonly ApplicationDbContext context = context;
        private readonly ICurrentUser currentUser = currentUser;
        public async Task<Result<List<MileageHistory>>> Handle(Query request, CancellationToken cancellationToken)
        {
            var familyId = await currentUser.GetFamilyIdAsync(cancellationToken);
            if (familyId is null)
                return Result.Fail<List<MileageHistory>>("Not authenticated.");

            var mileages = await context.MileageHistories
               .Where(m => m.VehicleId == request.VehicleId && m.Vehicle!.FamilyId == familyId)
               .OrderByDescending(m => m.RecordedDate)
               .ThenByDescending(m => m.Id)
               .Take(10)
               .AsNoTracking()
               .ToListAsync(cancellationToken);

            return mileages;
        }
    }
    public class EndPoint : IEndpointDefinition
    {
        public void MapEndpoints(WebApplication app)
        {
            app.MapGet("api/mileagehistory/vehicle/{vehicleId}", async (ISender sender, int vehicleId, CancellationToken cancellationToken) =>
            {
                var result = await sender.Send(new Query(vehicleId), cancellationToken);
                return result is not null ? Results.Ok(result) : Results.NotFound();
            }).RequireAuthorization();
        }
    }
}
