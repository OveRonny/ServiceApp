namespace serviceApp.Server.Features.Suppliers;

public static class GetAllSuppliers
{
    public record Query() : IQuery<List<Response>>;
    public record Response(int Id, string Name, string ContactEmail, string ContactPhone, string Address, string City, string PostalCode);

    public class Handler(ApplicationDbContext context) : IQueryHandler<Query, List<Response>>
    {
        private readonly ApplicationDbContext context = context;
        public async Task<Result<List<Response>>> Handle(Query request, CancellationToken cancellationToken)
        {
            var suppliers = await context.Suppliers
                .Select(s => new Response(s.Id, s.Name, s.ContactEmail, s.ContactPhone, s.Address, s.City, s.PostalCode))
                .ToListAsync(cancellationToken);
            return Result.Ok(suppliers);
        }
    }

    public class EndPoint : IEndpointDefinition
    {
        public void MapEndpoints(WebApplication app)
        {
            app.MapGet("api/supplier", async (ISender sender, CancellationToken cancellationToken) =>
            {
                var result = await sender.Send(new Query(), cancellationToken);
                if (result == null || result.Value == null)
                {
                    return Results.NotFound("No suppliers found.");
                }
                return Results.Ok(result.Value);
            }).RequireAuthorization(); ;
        }
    }
}


