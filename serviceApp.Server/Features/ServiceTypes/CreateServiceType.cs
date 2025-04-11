namespace serviceApp.Server.Features.ServiceTypes;

public static class CreateServiceType
{
    public record Command(string Name) : ICommand<Response>;
    public record Response(int Id, string Name);

    public class Handler(ApplicationDbContext context) : ICommandHandler<Command, Response>
    {
        private readonly ApplicationDbContext context = context;
        public async Task<Result<Response>> Handle(Command request, CancellationToken cancellationToken)
        {
            var serviceType = new ServiceType
            {
                Name = request.Name,
            };
            context.ServiceTypes.Add(serviceType);
            await context.SaveChangesAsync(cancellationToken);
            return new Response(serviceType.Id, serviceType.Name);
        }
    }
}

[ApiController]
[Route("api/service-type")]
public class CreateServiceTypeController(ISender sender) : ControllerBase
{
    private readonly ISender sender = sender;

    [HttpPost]
    public async Task<ActionResult<CreateServiceType.Response>> CreateServiceType([FromBody] CreateServiceType.Command command)
    {
        var result = await sender.Send(command);
        return Ok(result.Value);
    }
}
