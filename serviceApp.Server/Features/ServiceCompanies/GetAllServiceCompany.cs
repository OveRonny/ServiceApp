namespace serviceApp.Server.Features.ServiceCompanies;

public static class GetAllServiceCompany
{
    public record Query : IQuery<List<Response>>;
    public record Response(int Id, string Name);
    public class Handler(ApplicationDbContext context) : IQueryHandler<Query, List<Response>>
    {
        private readonly ApplicationDbContext context = context;
        public async Task<Result<List<Response>>> Handle(Query request, CancellationToken cancellationToken)
        {
            var serviceCompanies = await context.ServiceCompanies.ToListAsync(cancellationToken);
            var response = serviceCompanies.Select(sc => new Response(sc.Id, sc.Name)).ToList();
            return Result.Ok(response);
        }
    }
}

[ApiController]
[Route("api/service-company")]
public class GetAllServiceCompanyController(ISender sender) : ControllerBase
{
    private readonly ISender sender = sender;

    [HttpGet]
    public async Task<ActionResult<List<GetAllServiceCompany.Response>>> GetAllServiceCompany()
    {
        var result = await sender.Send(new GetAllServiceCompany.Query());
        return Ok(result.Value);
    }
}
