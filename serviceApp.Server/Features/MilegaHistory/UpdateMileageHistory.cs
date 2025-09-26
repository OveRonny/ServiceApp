using serviceApp.Server.Features.Autentication;

namespace serviceApp.Server.Features.MilegaHistory;

public static class UpdateMileageHistory
{
    public record Command(int Id, int Mileage, int? Hours) : ICommand;
    public class Handler(ApplicationDbContext context, ICurrentUser currentUser) : ICommandHandler<Command>
    {
        private readonly ApplicationDbContext context = context;
        private readonly ICurrentUser currentUser = currentUser;
        public async Task<Result> Handle(Command request, CancellationToken cancellationToken)
        {
            var familyId = await currentUser.GetFamilyIdAsync(cancellationToken);
            if (familyId is null)
                return Result.Fail("Not authenticated.");

            var mileage = await context.MileageHistories
                .Where(m => m.Id == request.Id && m.Vehicle!.FamilyId == familyId
                ).FirstOrDefaultAsync(cancellationToken);


            if (mileage is null)
                return Result.Fail("Mileage history not found.");
            mileage.Mileage = request.Mileage;
            mileage.Hours = request.Hours;
            await context.SaveChangesAsync(cancellationToken);
            return Result.Ok();
        }
    }

    public class EndPoint : IEndpointDefinition
    {
        public void MapEndpoints(WebApplication app)
        {
            app.MapPut("api/mileagehistory", async (ISender sender, Command command, CancellationToken cancellationToken) =>
            {
                var result = await sender.Send(command, cancellationToken);
                return result.Success ? Results.Ok() : Results.BadRequest(result.Error);
            }).RequireAuthorization();
        }
    }
}