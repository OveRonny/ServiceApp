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
            return Result.Ok(owner);
        }
    }

    public class EndPoint : IEndpointDefinition
    {
        public void MapEndpoints(WebApplication app)
        {
            app.MapGet("api/owner/{id}", async (ISender sender, int id, CancellationToken cancellationToken) =>
            {
                var result = await sender.Send(new Query(id), cancellationToken);
                if (result == null || result.Value == null)
                {
                    return Results.NotFound($"Owner with ID {id} not found.");
                }
                return Results.Ok(result.Value);
            }).RequireAuthorization(); ;
        }
    }
}


