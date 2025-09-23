namespace serviceApp.Server.Features.ServiceTypes;

public static class GetServiceTypeById
{
    public record Query(int Id) : IQuery<Response>;
    public record Response(int Id, string Name);
    public class Handler(ApplicationDbContext context) : IQueryHandler<Query, Response>
    {
        private readonly ApplicationDbContext context = context;
        public async Task<Result<Response>> Handle(Query request, CancellationToken cancellationToken)
        {
            var serviceType = await context.ServiceTypes.FindAsync(request.Id);
            if (serviceType == null)
            {
                return Result.Fail<Response>($"ServiceType with ID {request.Id} not found.");
            }
            return new Response(serviceType.Id, serviceType.Name);
        }
    }

    public class EndPoint : IEndpointDefinition
    {
        public void MapEndpoints(WebApplication app)
        {
            app.MapGet("api/service-type/{id}", async (ISender sender, int id, CancellationToken cancellationToken) =>
            {
                var result = await sender.Send(new Query(id), cancellationToken);
                if (result.Failure)
                {
                    return Results.NotFound(result.Error);
                }
                return Results.Ok(result.Value);
            }).RequireAuthorization(); ;
        }
    }
}

