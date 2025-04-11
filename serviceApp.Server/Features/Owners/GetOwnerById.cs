namespace serviceApp.Server.Features.Owners;

public static class GetOwnerById
{
    public record Query(int Id) : IQuery<Response>;
    public record Response(int Id, string FirstName, string LastName, string PhoneNumber, string Email, string Address, string PostalCode, string City);
    public class Handler(ApplicationDbContext context) : IQueryHandler<Query, Response>
    {
        private readonly ApplicationDbContext context = context;
        public async Task<Result<Response>> Handle(Query request, CancellationToken cancellationToken)
        {
            var owner = await context.Owner
                .Where(o => o.Id == request.Id)
                .Select(o => new Response(o.Id, o.FirstName, o.LastName, o.PhoneNumber, o.Email, o.Address, o.PostalCode, o.City))
                .FirstOrDefaultAsync(cancellationToken);
            if (owner == null)
            {
                return Result.Fail<Response>($"Owner with ID {request.Id} not found.");
            }
            return owner;
        }
    }
}

[ApiController]
[Route("api/owner")]
public class GetOwnerByIdController(ISender sender) : ControllerBase
{
    private readonly ISender sender = sender;

    [HttpGet("{id}")]
    public async Task<ActionResult<GetOwnerById.Response>> GetOwnerById(int id)
    {
        var result = await sender.Send(new GetOwnerById.Query(id));
        if (result.Failure)
        {
            return NotFound(result.Error);
        }
        return Ok(result);
    }
}

