namespace serviceApp.Server.Features.Suppliers;

public static class GetSupplierById
{
    public record Query(int Id) : IQuery<Response>;
    public record Response(int Id, string Name, string ContactEmail, string ContactPhone, string Address, string City, string PostalCode);
    public class Handler(ApplicationDbContext context) : IQueryHandler<Query, Response>
    {
        private readonly ApplicationDbContext context = context;
        public async Task<Result<Response>> Handle(Query request, CancellationToken cancellationToken)
        {
            var supplier = await context.Suppliers
                .Where(s => s.Id == request.Id)
                .Select(s => new Response(s.Id, s.Name, s.ContactEmail, s.ContactPhone, s.Address, s.City, s.PostalCode))
                .FirstOrDefaultAsync(cancellationToken);
            if (supplier == null)
            {
                return Result.Fail<Response>($"Supplier with ID {request.Id} not found.");
            }
            return supplier;
        }
    }

    public class EndPoint : IEndpointDefinition
    {
        public void MapEndpoints(WebApplication app)
        {
            app.MapGet("api/supplier/{id}", async (ISender sender, int id, CancellationToken cancellationToken) =>
            {
                var result = await sender.Send(new Query(id), cancellationToken);
                if (result == null || result.Value == null)
                {
                    return Results.NotFound($"Supplier with ID {id} not found.");
                }
                return Results.Ok(result.Value);
            }).RequireAuthorization(); ;
        }
    }
}



