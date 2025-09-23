namespace serviceApp.Server.Features.ServiceTypes;

public static class GetAllServiceType
{
    public record Query : IQuery<List<Response>>;

    public record Response(int Id, string Name);

    public class Handler(ApplicationDbContext context) : IQueryHandler<Query, List<Response>>
    {
        private readonly ApplicationDbContext context = context;
        public async Task<Result<List<Response>>> Handle(Query request, CancellationToken cancellationToken)
        {
            var serviceTypes = await context.ServiceTypes.ToListAsync(cancellationToken);
            var response = serviceTypes.Select(st => new Response(st.Id, st.Name)).ToList();
            return Result.Ok(response);
        }
    }

    public class EndPoint : IEndpointDefinition
    {
        public void MapEndpoints(WebApplication app)
        {
            app.MapGet("api/service-type", async (ISender sender, CancellationToken cancellationToken) =>
            {
                var result = await sender.Send(new Query(), cancellationToken);
                return Results.Ok(result.Value);
            }).RequireAuthorization(); ;
        }
    }
}


