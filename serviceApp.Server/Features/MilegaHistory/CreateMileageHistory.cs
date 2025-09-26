using serviceApp.Server.Features.Autentication;

namespace serviceApp.Server.Features.MilegaHistory;

public static class CreateMileageHistory
{
    public record Command(int VehicleId, int Mileage, DateTime RecordedDate) : ICommand<Response>;

    public record Response(int Id, int VehicleId, int Mileage, DateTime RecordedDate, Guid FamilyId);

    public class Handler(ApplicationDbContext context, ICurrentUser currentUser) : ICommandHandler<Command, Response>
    {
        private readonly ApplicationDbContext context = context;
        private readonly ICurrentUser currentUser = currentUser;

        public async Task<Result<Response>> Handle(Command request, CancellationToken cancellationToken)
        {
            var familyId = await currentUser.GetFamilyIdAsync(cancellationToken);
            if (familyId is null)
                return Result.Fail<Response>("Not authenticated.");

            var mileageHistory = new MileageHistory
            {
                VehicleId = request.VehicleId,
                Mileage = request.Mileage,
                RecordedDate = request.RecordedDate,
                Type = MileageHistory.MileageType.Kilometerstand
            };
            context.MileageHistories.Add(mileageHistory);
            await context.SaveChangesAsync(cancellationToken);
            return new Response(mileageHistory.Id, mileageHistory.VehicleId, mileageHistory.Mileage, mileageHistory.RecordedDate, mileageHistory.FamilyId);
        }
    }

    public class EndPoint : IEndpointDefinition
    {
        public void MapEndpoints(WebApplication app)
        {
            app.MapPost("api/mileagehistory", async (ISender sender, Command command, CancellationToken cancellationToken) =>
            {
                var result = await sender.Send(command, cancellationToken);
                return Results.Ok(result.Value);
            }).RequireAuthorization();
        }
    }

}
