namespace serviceApp.Server.Features.ServiceCompanies;

public static class DeleteServiceCompany
{
    public record class Command(int Id) : ICommand<bool>;
    public class Handler(ApplicationDbContext context) : ICommandHandler<Command, bool>
    {
        private readonly ApplicationDbContext context = context;
        public async Task<Result<bool>> Handle(Command request, CancellationToken cancellationToken)
        {
            var serviceCompany = await context.ServiceCompanies.FindAsync(request.Id);
            if (serviceCompany == null)
            {
                return Result.Fail<bool>($"Service company with ID {request.Id} not found.");
            }
            context.ServiceCompanies.Remove(serviceCompany);
            await context.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}

[ApiController]
[Route("api/service-company")]
public class DeleteServiceCompanyController(ISender sender) : ControllerBase
{
    private readonly ISender sender = sender;

    [HttpDelete("{id}")]
    public async Task<ActionResult<bool>> DeleteServiceCompany(int id)
    {
        var command = new DeleteServiceCompany.Command(id);
        var result = await sender.Send(command);
        if (result.Failure)
        {
            return NotFound(result.Error);
        }
        return true;
    }
}
