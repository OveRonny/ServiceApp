

namespace serviceApp.Server.Features.ServiceCompanies;

public static class CreateServiceCompany
{
    public record Command(string Name) : ICommand<Response>;

    public record Response(int Id, string Name);

    public class Handler(ApplicationDbContext context) : ICommandHandler<Command, Response>
    {
        private readonly ApplicationDbContext context = context;

        public async Task<Result<Response>> Handle(Command request, CancellationToken cancellationToken)
        {
            var serviceCompany = new ServiceCompany
            {
                Name = request.Name
            };

            context.ServiceCompanies.Add(serviceCompany);
            await context.SaveChangesAsync(cancellationToken);
            return new Response(serviceCompany.Id, serviceCompany.Name);
        }
    }

    public class EndPoint : IEndpointDefinition
    {
        public void MapEndpoints(WebApplication app)
        {
            app.MapPost("api/service-company", async (ISender sender, CreateServiceCompany.Command command, CancellationToken cancellationToken) =>
            {
                var result = await sender.Send(command, cancellationToken);
                return Results.Ok(result.Value);
            }).RequireAuthorization(); ;
        }
    }
}


