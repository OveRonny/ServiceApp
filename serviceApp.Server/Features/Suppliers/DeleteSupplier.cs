namespace serviceApp.Server.Features.Suppliers;

public static class DeleteSupplier
{
    public record Command(int Id) : ICommand<bool>;
    public class Handler(ApplicationDbContext context) : ICommandHandler<Command, bool>
    {
        private readonly ApplicationDbContext context = context;
        public async Task<Result<bool>> Handle(Command request, CancellationToken cancellationToken)
        {
            var supplier = await context.Suppliers.FindAsync(request.Id);
            if (supplier == null)
            {
                return false;
            }
            context.Suppliers.Remove(supplier);
            await context.SaveChangesAsync(cancellationToken);
            return true;
        }
    }

    public class EndPoint : IEndpointDefinition
    {
        public void MapEndpoints(WebApplication app)
        {
            app.MapDelete("api/supplier/{id}", async (ISender sender, int id, CancellationToken cancellationToken) =>
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


