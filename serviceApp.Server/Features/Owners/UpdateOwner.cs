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

    public class EndPoint : IEndpointDefinition
    {
        public void MapEndpoints(WebApplication app)
        {
            app.MapPut("api/owner/{id}", async (ISender sender, int id, UpdateOwner.Command command, CancellationToken cancellationToken) =>
            {
                if (id != command.Id)
                {
                    return Results.BadRequest("ID in the URL does not match ID in the request body.");
                }
                var result = await sender.Send(command, cancellationToken);
                if (result.Failure)
                {
                    return Results.NotFound(result.Error);
                }
                return Results.Ok(result.Value);
            }).RequireAuthorization();
        }
    }
}


