using serviceApp.Server.Features.Autentication;

namespace serviceApp.Server.Features.MilegaHistory;

public static class GetLastMilage
{
    public record Query(int VehicleId) : IQuery<MileageHistory>;

    public class Handler(ApplicationDbContext context, ICurrentUser currentUser) : IQueryHandler<Query, MileageHistory>
    {
        private readonly ApplicationDbContext context = context;
        private readonly ICurrentUser currentUser = currentUser;

        public async Task<Result<MileageHistory>> Handle(Query request, CancellationToken cancellationToken)
        {
            var familyId = await currentUser.GetFamilyIdAsync(cancellationToken);
            if (familyId is null)
                return Result.Fail<MileageHistory>("Not authenticated.");

            var mileage = await context.MileageHistories
                 .Where(m => m.VehicleId == request.VehicleId && m.Vehicle!.FamilyId == familyId)
                 .OrderByDescending(m => m.RecordedDate)
                 .FirstOrDefaultAsync(cancellationToken);


            return mileage is null
                ? Result.Fail<MileageHistory>("No mileage history found.")
                : Result.Ok(mileage);
        }
    }

    public class EndPoint : IEndpointDefinition
    {
        public void MapEndpoints(WebApplication app)
        {
            app.MapGet("api/mileagehistory/last/{vehicleId}", async (ISender sender, int vehicleId, CancellationToken cancellationToken) =>
            {
                var result = await sender.Send(new Query(vehicleId), cancellationToken);
                if (!result.Success)
                    return Results.NotFound(result.Error);
                return Results.Ok(result.Value);
            }).RequireAuthorization();
        }
    }

}
