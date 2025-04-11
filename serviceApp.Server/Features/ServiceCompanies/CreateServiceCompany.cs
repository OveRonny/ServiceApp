

namespace serviceApp.Server.Features.ServiceCompanies;

public static class CreateServiceCompany
{
    public record Command(string Name) : ICommand<Response>;

    public record Response(int Id, string Name);

    public class Handler(ApplicationDbContext context) : ICommandHandler<Command, Response>
    {
        private readonly ApplicationDbContext context = context;

        public async Task<Result<Response>> Handle(Command request, CancellationToken cancellationToken)
        {
            var serviceCompany = new ServiceCompany
            {
                Name = request.Name
            };

            context.ServiceCompanies.Add(serviceCompany);
            await context.SaveChangesAsync(cancellationToken);
            return new Response(serviceCompany.Id, serviceCompany.Name);
        }
    }
}

[ApiController]
[Route("api/service-company")]
public class CreateServiceCompanyController(ISender sender) : ControllerBase
{
    private readonly ISender sender = sender;

    [HttpPost]
    public async Task<ActionResult<CreateServiceCompany.Response>> CreateServiceCompany([FromBody] CreateServiceCompany.Command command)
    {
        var result = await sender.Send(command);
        return Ok(result.Value);
    }
}
