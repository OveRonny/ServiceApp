namespace serviceApp.Server.Features.ServiceCompanies;

public static class DeleteServiceCompany
{
    public record class Command(int Id) : ICommand<bool>;
    public class Handler(ApplicationDbContext context) : ICommandHandler<Command, bool>
    {
        private readonly ApplicationDbContext context = context;
        public async Task<Result<bool>> Handle(Command request, CancellationToken cancellationToken)
        {
            var serviceCompany = await context.ServiceCompanies.FindAsync(request.Id);
            if (serviceCompany == null)
            {
                return Result.Fail<bool>($"Service company with ID {request.Id} not found.");
            }
            context.ServiceCompanies.Remove(serviceCompany);
            await context.SaveChangesAsync(cancellationToken);
            return true;
        }
    }

    public class EndPoint : IEndpointDefinition
    {
        public void MapEndpoints(WebApplication app)
        {
            app.MapDelete("api/service-company/{id}", async (ISender sender, int id, CancellationToken cancellationToken) =>
            {
                var result = await sender.Send(new Command(id), cancellationToken);
                if (result.Failure)
                {
                    return false;
                }
                return true;
            }).RequireAuthorization(); ;
        }
    }
}


