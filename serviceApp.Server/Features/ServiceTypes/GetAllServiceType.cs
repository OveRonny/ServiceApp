namespace serviceApp.Server.Features.ServiceTypes;

public static class GetAllServiceType
{
    public record Query : IQuery<List<Response>>;

    public record Response(int Id, string Name);

    public class Handler(ApplicationDbContext context) : IQueryHandler<Query, List<Response>>
    {
        private readonly ApplicationDbContext context = context;
        public async Task<Result<List<Response>>> Handle(Query request, CancellationToken cancellationToken)
        {
            var serviceTypes = await context.ServiceTypes.ToListAsync(cancellationToken);
            var response = serviceTypes.Select(st => new Response(st.Id, st.Name)).ToList();
            return Result.Ok(response);
        }
    }
}

[ApiController]
[Route("api/service-type")]
public class GetAllServiceTypeController(ISender sender) : ControllerBase
{
    private readonly ISender sender = sender;

    [HttpGet]
    public async Task<ActionResult<List<GetAllServiceType.Response>>> GetAllServiceTypes()
    {
        var result = await sender.Send(new GetAllServiceType.Query());
        return Ok(result);
    }
}
