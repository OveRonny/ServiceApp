namespace serviceApp.Server.Features.ServiceTypes;

public static class DeleteServiceType
{
    public record Command(int Id) : ICommand<bool>;

    public class Handler(ApplicationDbContext context) : ICommandHandler<Command, bool>
    {
        private readonly ApplicationDbContext context = context;
        public async Task<Result<bool>> Handle(Command request, CancellationToken cancellationToken)
        {
            var serviceType = await context.ServiceTypes.FindAsync(request.Id);
            if (serviceType == null)
            {
                return false;
            }
            context.ServiceTypes.Remove(serviceType);
            await context.SaveChangesAsync(cancellationToken);
            return true;
        }
    }

    public class EndPoint : IEndpointDefinition
    {
        public void MapEndpoints(WebApplication app)
        {
            app.MapDelete("api/service-type/{id}", async (ISender sender, int id, CancellationToken cancellationToken) =>
            {
                var result = await sender.Send(new Command(id), cancellationToken);
                if (result.Failure)
                {
                    return false;
                }
                return true;
            });
        }
    }
}


