namespace serviceApp.Server.Features.Suppliers;

public static class DeleteSupplier
{
    public record Command(int Id) : ICommand<bool>;
    public class Handler(ApplicationDbContext context) : ICommandHandler<Command, bool>
    {
        private readonly ApplicationDbContext context = context;
        public async Task<Result<bool>> Handle(Command request, CancellationToken cancellationToken)
        {
            var supplier = await context.Suppliers.FindAsync(request.Id);
            if (supplier == null)
            {
                return false;
            }
            context.Suppliers.Remove(supplier);
            await context.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}

[ApiController]
[Route("api/supplier")]
public class DeleteSupplierController(ISender sender) : ControllerBase
{
    private readonly ISender sender = sender;

    [HttpDelete("{id}")]
    public async Task<ActionResult<bool>> DeleteSupplier(int id)
    {
        var result = await sender.Send(new DeleteSupplier.Command(id));
        if (result.Failure)
        {
            return NotFound(result.Error);
        }
        return true;
    }
}
