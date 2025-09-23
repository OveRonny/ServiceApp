namespace serviceApp.Server.Features.ServiceTypes;

public static class UpdateServiceType
{
    public record Command(int Id, string Name) : ICommand<Response>;
    public record Response(int Id, string Name);
    public class Handler(ApplicationDbContext context) : ICommandHandler<Command, Response>
    {
        private readonly ApplicationDbContext context = context;
        public async Task<Result<Response>> Handle(Command request, CancellationToken cancellationToken)
        {
            var serviceType = await context.ServiceTypes.FindAsync(request.Id);
            if (serviceType == null)
            {
                return Result.Fail<Response>($"ServiceType with ID {request.Id} not found.");
            }
            serviceType.Name = request.Name;
            await context.SaveChangesAsync(cancellationToken);
            return new Response(serviceType.Id, serviceType.Name);
        }
    }

    public class EndPoint : IEndpointDefinition
    {
        public void MapEndpoints(WebApplication app)
        {
            app.MapPut("api/service-type/{id}", async (ISender sender, int id, Command command, CancellationToken cancellationToken) =>
            {
                if (id != command.Id)
                {
                    return Results.BadRequest("ServiceType ID mismatch.");
                }
                var result = await sender.Send(command, cancellationToken);
                if (result.Failure)
                {
                    return Results.NotFound(result.Error);
                }
                return Results.Ok(result.Value);
            }).RequireAuthorization(); ;
        }
    }
}


