namespace serviceApp.Server.Features.ServiceTypes;

public static class GetServiceTypeById
{
    public record Query(int Id) : IQuery<Response>;
    public record Response(int Id, string Name);
    public class Handler(ApplicationDbContext context) : IQueryHandler<Query, Response>
    {
        private readonly ApplicationDbContext context = context;
        public async Task<Result<Response>> Handle(Query request, CancellationToken cancellationToken)
        {
            var serviceType = await context.ServiceTypes.FindAsync(request.Id);
            if (serviceType == null)
            {
                return Result.Fail<Response>($"ServiceType with ID {request.Id} not found.");
            }
            return new Response(serviceType.Id, serviceType.Name);
        }
    }
}

[ApiController]
[Route("api/service-type")]
public class GetServiceTypeByIdController(ISender sender) : ControllerBase
{
    private readonly ISender sender = sender;

    [HttpGet("{id}")]
    public async Task<ActionResult<GetServiceTypeById.Response>> GetServiceTypeById(int id)
    {
        var result = await sender.Send(new GetServiceTypeById.Query(id));
        if (result.Failure)
        {
            return NotFound(result.Error);
        }
        return Ok(result.Value);
    }
}
