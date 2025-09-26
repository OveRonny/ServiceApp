using serviceApp.Server.Features.Autentication;

namespace serviceApp.Server.Features.MilegaHistory;

public static class GetMileageHistoryById
{
    public record Query(int Id) : IQuery<MileageHistory?>;
    public class Handler(ApplicationDbContext context, ICurrentUser currentUser) : IQueryHandler<Query, MileageHistory?>
    {
        private readonly ApplicationDbContext context = context;
        private readonly ICurrentUser currentUser = currentUser;
        public async Task<Result<MileageHistory?>> Handle(Query request, CancellationToken cancellationToken)
        {
            var familyId = await currentUser.GetFamilyIdAsync(cancellationToken);
            if (familyId is null)
                return Result.Fail<MileageHistory?>("Not authenticated.");

            var mileage = await context.MileageHistories
                .Where(m => m.Id == request.Id && m.Vehicle!.FamilyId == familyId
                ).FirstOrDefaultAsync(cancellationToken);
            return mileage;
        }
    }

    public class EndPoint : IEndpointDefinition
    {
        public void MapEndpoints(WebApplication app)
        {
            app.MapGet("api/mileagehistory/{id}", async (ISender sender, int id, CancellationToken cancellationToken) =>
            {
                var result = await sender.Send(new Query(id), cancellationToken);
                return result is not null ? Results.Ok(result) : Results.NotFound();
            }).RequireAuthorization();
        }
    }
}
