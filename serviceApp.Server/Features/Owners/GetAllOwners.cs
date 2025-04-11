namespace serviceApp.Server.Features.Owners;

public static class GetAllOwners
{
    public record Query() : IQuery<Response>;
    public record Response(List<OwnerDto> Owners);
    public record OwnerDto(int Id, string FirstName, string LastName, string PhoneNumber, string Email, string Address, string PostalCode, string City);
    public class Handler(ApplicationDbContext context) : IQueryHandler<Query, Response>
    {
        private readonly ApplicationDbContext context = context;
        public async Task<Result<Response>> Handle(Query request, CancellationToken cancellationToken)
        {
            var owners = await context.Owner
                .Select(o => new OwnerDto(o.Id, o.FirstName, o.LastName, o.PhoneNumber, o.Email, o.Address, o.PostalCode, o.City))
                .ToListAsync(cancellationToken);
            return new Response(owners);
        }
    }
}

[ApiController]
[Route("api/owner")]
public class GetAllOwnerController(ISender sender) : ControllerBase
{
    private readonly ISender sender = sender;

    [HttpGet]
    public async Task<ActionResult<GetAllOwners.Response>> GetAllOwners()
    {
        var result = await sender.Send(new GetAllOwners.Query());
        return Ok(result);
    }
}
