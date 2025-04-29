namespace serviceApp.Server.Features.ServiceCompanies;

public static class GetAllServiceCompany
{
    public record Query : IQuery<List<Response>>;
    public record Response(int Id, string Name);
    public class Handler(ApplicationDbContext context) : IQueryHandler<Query, List<Response>>
    {
        private readonly ApplicationDbContext context = context;
        public async Task<Result<List<Response>>> Handle(Query request, CancellationToken cancellationToken)
        {
            var serviceCompanies = await context.ServiceCompanies.ToListAsync(cancellationToken);
            var response = serviceCompanies.Select(sc => new Response(sc.Id, sc.Name)).ToList();
            return Result.Ok(response);
        }
    }

    public class EndPoint : IEndpointDefinition
    {
        public void MapEndpoints(WebApplication app)
        {
            app.MapGet("api/service-company", async (ISender sender, CancellationToken cancellationToken) =>
            {
                var result = await sender.Send(new Query(), cancellationToken);
                return Results.Ok(result.Value);
            });
        }
    }
}

