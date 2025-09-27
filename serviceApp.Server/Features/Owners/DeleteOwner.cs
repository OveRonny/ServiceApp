namespace serviceApp.Server.Features.Owners;

public static class DeleteOwner
{
    public record Command(int Id) : ICommand<bool>;
    public class Handler(ApplicationDbContext context) : ICommandHandler<Command, bool>
    {
        private readonly ApplicationDbContext context = context;
        public async Task<Result<bool>> Handle(Command request, CancellationToken cancellationToken)
        {
            var owner = await context.Owner.FindAsync(request.Id);
            if (owner == null)
            {
                return Result.Fail<bool>($"Owner with ID {request.Id} not found.");
            }
            context.Owner.Remove(owner);
            await context.SaveChangesAsync(cancellationToken);
            return true;
        }
    }

    public class EndPoint : IEndpointDefinition
    {
        public void MapEndpoints(WebApplication app)
        {
            app.MapDelete("api/owner/{id}", async (ISender sender, int id, CancellationToken cancellationToken) =>
            {
                var result = await sender.Send(new Command(id), cancellationToken);
                if (result.Failure)
                {
                    return Results.NotFound(result.Error);
                }
                return Results.Ok(true);
            }).RequireAuthorization();
        }
    }
}


