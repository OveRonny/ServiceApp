namespace serviceApp.Server.Features.ServiceTypes;

public static class CreateServiceType
{
    public record Command(string Name) : ICommand<Response>;
    public record Response(int Id, string Name);

    public class Handler(ApplicationDbContext context) : ICommandHandler<Command, Response>
    {
        private readonly ApplicationDbContext context = context;
        public async Task<Result<Response>> Handle(Command request, CancellationToken cancellationToken)
        {
            var serviceType = new ServiceType
            {
                Name = request.Name,
            };
            context.ServiceTypes.Add(serviceType);
            await context.SaveChangesAsync(cancellationToken);
            return new Response(serviceType.Id, serviceType.Name);
        }
    }

    public class EndPoint : IEndpointDefinition
    {
        public void MapEndpoints(WebApplication app)
        {
            app.MapPost("api/service-type", async (ISender sender, CreateServiceType.Command command, CancellationToken cancellationToken) =>
            {
                var result = await sender.Send(command, cancellationToken);
                return Results.Ok(result.Value);
            });
        }
    }
}


