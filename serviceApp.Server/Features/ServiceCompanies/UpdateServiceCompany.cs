namespace serviceApp.Server.Features.ServiceCompanies;

public static class UpdateServiceCompany
{
    public record Command(int Id, string Name) : ICommand<Response>;
    public record Response(int Id, string Name);

    public class Handler(ApplicationDbContext context) : ICommandHandler<Command, Response>
    {
        private readonly ApplicationDbContext context = context;
        public async Task<Result<Response>> Handle(Command request, CancellationToken cancellationToken)
        {
            var serviceCompany = await context.ServiceCompanies.FindAsync(request.Id);
            if (serviceCompany == null)
            {
                return Result.Fail<Response>("Service company not found");
            }
            serviceCompany.Name = request.Name;
            await context.SaveChangesAsync(cancellationToken);
            return new Response(serviceCompany.Id, serviceCompany.Name);
        }
    }
}


[ApiController]
[Route("api/service-company")]
public class UpdateServiceCompanyController(ISender sender) : ControllerBase
{
    private readonly ISender sender = sender;

    [HttpPut("{id}")]
    public async Task<ActionResult<UpdateServiceCompany.Response>> UpdateServiceCompany(int id, [FromBody] UpdateServiceCompany.Command command)
    {
        if (id != command.Id)
        {
            return BadRequest("Service company ID mismatch.");
        }
        var result = await sender.Send(command);
        if (result.Failure)
        {
            return NotFound(result.Error);
        }
        return Ok(result.Value);
    }
}
