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
}

[ApiController]
[Route("api/owner")]
public class DeleteOwnerController(ISender sender) : ControllerBase
{
    private readonly ISender sender = sender;

    [HttpDelete("{id}")]
    public async Task<ActionResult<bool>> DeleteOwner(int id)
    {
        var result = await sender.Send(new DeleteOwner.Command(id));
        if (result.Failure)
        {
            return NotFound(result.Error);
        }
        return true;
    }
}
