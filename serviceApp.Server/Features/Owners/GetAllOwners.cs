namespace serviceApp.Server.Features.Owners;

public static class GetAllOwners
{
    public record Query() : IQuery<Response>;
    public record Response(List<OwnerDto> Owners);
    public record OwnerDto(int Id, string FirstName, string LastName, string PhoneNumber, string Email,
        string Address, string PostalCode, string City);
    public class Handler(ApplicationDbContext context) : IQueryHandler<Query, Response>
    {
        private readonly ApplicationDbContext context = context;
        public async Task<Result<Response>> Handle(Query request, CancellationToken cancellationToken)
        {
            var owners = await context.Owner
                .Select(o => new OwnerDto(o.Id, o.FirstName, o.LastName, o.PhoneNumber, o.Email, o.Address, o.PostalCode, o.City))
                .ToListAsync(cancellationToken);
            return Result.Ok(new Response(owners));
        }
    }

    public class EndPoint : IEndpointDefinition
    {
        public void MapEndpoints(WebApplication app)
        {
            app.MapGet("api/owner", async (ISender sender, CancellationToken cancellationToken) =>
            {
                var result = await sender.Send(new Query(), cancellationToken);
                if (result == null || result.Value == null)
                {
                    return Results.NotFound("No owners found.");
                }

                return Results.Ok(result.Value.Owners);
            }).RequireAuthorization(); ;
        }
    }
}

