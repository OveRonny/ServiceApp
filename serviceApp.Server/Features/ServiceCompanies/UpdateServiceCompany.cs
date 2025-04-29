namespace serviceApp.Server.Features.ServiceCompanies;

public static class UpdateServiceCompany
{
    public record Command(int Id, string Name) : ICommand<Response>;
    public record Response(int Id, string Name);

    public class Handler(ApplicationDbContext context) : ICommandHandler<Command, Response>
    {
        private readonly ApplicationDbContext context = context;
        public async Task<Result<Response>> Handle(Command request, CancellationToken cancellationToken)
        {
            var serviceCompany = await context.ServiceCompanies.FindAsync(request.Id);
            if (serviceCompany == null)
            {
                return Result.Fail<Response>("Service company not found");
            }
            serviceCompany.Name = request.Name;
            await context.SaveChangesAsync(cancellationToken);
            return new Response(serviceCompany.Id, serviceCompany.Name);
        }
    }

    public class EndPoint : IEndpointDefinition
    {
        public void MapEndpoints(WebApplication app)
        {
            app.MapPut("api/service-company/{id}", async (ISender sender, int id, Command command, CancellationToken cancellationToken) =>
            {
                if (id != command.Id)
                {
                    return Results.BadRequest("Service company ID mismatch.");
                }
                var result = await sender.Send(command, cancellationToken);
                if (result.Failure)
                {
                    return Results.NotFound(result.Error);
                }
                return Results.Ok(result.Value);
            });
        }
    }
}



