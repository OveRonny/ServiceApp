namespace serviceApp.Server.Features.ServiceCompanies;

public static class GetServiceCompanyById
{
    public record class Query(int Id) : IQuery<Response>;
    public record Response(int Id, string Name);
    public class Handler(ApplicationDbContext context) : IQueryHandler<Query, Response>
    {
        private readonly ApplicationDbContext context = context;
        public async Task<Result<Response>> Handle(Query request, CancellationToken cancellationToken)
        {
            var serviceCompany = await context.ServiceCompanies.FindAsync(request.Id);
            if (serviceCompany == null)
            {
                return Result.Fail<Response>($"Service company with ID {request.Id} not found.");
            }
            var response = new Response(serviceCompany.Id, serviceCompany.Name);
            return Result.Ok(response);
        }
    }
}

[ApiController]
[Route("api/service-company")]
public class GetServiceCompanyByIdController(ISender sender) : ControllerBase
{
    private readonly ISender sender = sender;

    [HttpGet("{id}")]
    public async Task<ActionResult<GetServiceCompanyById.Response>> GetServiceCompanyById(int id)
    {
        var result = await sender.Send(new GetServiceCompanyById.Query(id));
        if (result.Failure)
        {
            return NotFound(result.Error);
        }
        return Ok(result.Value);
    }
}
