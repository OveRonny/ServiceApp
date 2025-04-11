namespace serviceApp.Server.Features.Owners;

public static class UpdateOwner
{
    public record Command(int Id, string FirstName, string LastName, string PhoneNumber, string Email, string Address, string PostalCode, string City) : ICommand<Response>;
    public record Response(int Id, string FirstName, string LastName, string PhoneNumber, string Email, string Address, string PostalCode, string City);
    public class Handler(ApplicationDbContext context) : ICommandHandler<Command, Response>
    {
        private readonly ApplicationDbContext context = context;
        public async Task<Result<Response>> Handle(Command request, CancellationToken cancellationToken)
        {
            var owner = await context.Owner.FindAsync(request.Id);
            if (owner == null)
            {
                return Result.Fail<Response>($"Owner with ID {request.Id} not found.");
            }

            owner.FirstName = request.FirstName;
            owner.LastName = request.LastName;
            owner.PhoneNumber = request.PhoneNumber;
            owner.Email = request.Email;
            owner.Address = request.Address;
            owner.PostalCode = request.PostalCode;
            owner.City = request.City;

            await context.SaveChangesAsync(cancellationToken);
            return new Response(owner.Id, owner.FirstName, owner.LastName, owner.PhoneNumber, owner.Email, owner.Address, owner.PostalCode, owner.City);
        }
    }
}

[ApiController]
[Route("api/owner")]
public class UpdateOwnerController(ISender sender) : ControllerBase
{
    private readonly ISender sender = sender;

    [HttpPut("{id}")]
    public async Task<ActionResult<UpdateOwner.Response>> UpdateOwner(int id, [FromBody] UpdateOwner.Command command)
    {
        if (id != command.Id)
        {
            return BadRequest("ID in the URL does not match ID in the request body.");
        }
        var result = await sender.Send(command);
        if (result.Failure)
        {
            return NotFound(result.Error);
        }
        return Ok(result);
    }
}
