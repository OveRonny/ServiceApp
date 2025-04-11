namespace serviceApp.Server.Features.ServiceTypes;

public static class UpdateServiceType
{
    public record Command(int Id, string Name) : ICommand<Response>;
    public record Response(int Id, string Name);
    public class Handler(ApplicationDbContext context) : ICommandHandler<Command, Response>
    {
        private readonly ApplicationDbContext context = context;
        public async Task<Result<Response>> Handle(Command request, CancellationToken cancellationToken)
        {
            var serviceType = await context.ServiceTypes.FindAsync(request.Id);
            if (serviceType == null)
            {
                return Result.Fail<Response>($"ServiceType with ID {request.Id} not found.");
            }
            serviceType.Name = request.Name;
            await context.SaveChangesAsync(cancellationToken);
            return new Response(serviceType.Id, serviceType.Name);
        }
    }
}

[ApiController]
[Route("api/service-type")]
public class UpdateServiceTypeController(ISender sender) : ControllerBase
{
    private readonly ISender sender = sender;
    [HttpPut("{id}")]
    public async Task<ActionResult<UpdateServiceType.Response>> UpdateServiceType(int id, [FromBody] UpdateServiceType.Command command)
    {
        if (id != command.Id)
        {
            return BadRequest("ServiceType ID mismatch.");
        }
        var result = await sender.Send(command);
        if (result.Failure)
        {
            return NotFound(result.Error);
        }
        return Ok(result.Value);
    }
}
