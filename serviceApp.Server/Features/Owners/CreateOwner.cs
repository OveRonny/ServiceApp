namespace serviceApp.Server.Features.Owners;

public static class CreateOwner
{
    public record Command(string FirstName, string LastName, string PhoneNumber, string Email, string Address, string PostalCode, string City) : ICommand<Response>;

    public record Response(int Id, string FirstName, string LastName, string PhoneNumber, string Email, string Address, string PostalCode, string City);

    public class Handler(ApplicationDbContext context) : ICommandHandler<Command, Response>
    {
        private readonly ApplicationDbContext context = context;

        public async Task<Result<Response>> Handle(Command request, CancellationToken cancellationToken)
        {
            var owner = new Owner
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                PhoneNumber = request.PhoneNumber,
                Email = request.Email,
                Address = request.Address,
                PostalCode = request.PostalCode,
                City = request.City
            };
            context.Owner.Add(owner);
            await context.SaveChangesAsync(cancellationToken);
            return new Response(owner.Id, owner.FirstName, owner.LastName, owner.PhoneNumber, owner.Email, owner.Address, owner.PostalCode, owner.City);
        }
    }
}

[ApiController]
[Route("api/owner")]
public class CreateOwnerController(ISender sender) : ControllerBase
{
    private readonly ISender sender = sender;

    [HttpPost]
    public async Task<ActionResult<CreateOwner.Response>> CreateOwner([FromBody] CreateOwner.Command command)
    {
        var result = await sender.Send(command);
        return Ok(result);
    }
}
