using serviceApp.Server.Features.Autentication;

namespace serviceApp.Server.Features.ServiceTypes;

public static class CreateServiceType
{
    public record Command(string Name) : ICommand<Response>;
    public record Response(int Id, string Name);

    public class Handler(ApplicationDbContext context, ICurrentUser currentUser) : ICommandHandler<Command, Response>
    {
        private readonly ApplicationDbContext context = context;
        private readonly ICurrentUser currentUser = currentUser;

        public async Task<Result<Response>> Handle(Command request, CancellationToken cancellationToken)
        {

            if (!currentUser.IsAuthenticated || currentUser.FamilyId is null)
                return Result.Fail<Response>("Not authenticated.");

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


