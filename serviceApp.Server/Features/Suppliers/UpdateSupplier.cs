namespace serviceApp.Server.Features.Suppliers;

public static class UpdateSupplier
{
    public record Command(int Id, string Name, string ContactEmail, string ContactPhone, string Address, string City, string PostalCode) : ICommand<Response>;
    public record Response(int Id, string Name, string ContactEmail, string ContactPhone, string Address, string City, string PostalCode, DateTime UpdatedAt);
    public class Handler(ApplicationDbContext context) : ICommandHandler<Command, Response>
    {
        private readonly ApplicationDbContext context = context;
        public async Task<Result<Response>> Handle(Command request, CancellationToken cancellationToken)
        {
            var supplier = await context.Suppliers.FindAsync(request.Id);
            if (supplier == null)
            {
                return Result.Fail<Response>($"Supplier with ID {request.Id} not found.");
            }
            supplier.Name = request.Name;
            supplier.ContactEmail = request.ContactEmail;
            supplier.ContactPhone = request.ContactPhone;
            supplier.Address = request.Address;
            supplier.City = request.City;
            supplier.PostalCode = request.PostalCode;
            supplier.UpdatedAt = DateTime.Now;
            await context.SaveChangesAsync(cancellationToken);
            return new Response(supplier.Id, supplier.Name, supplier.ContactEmail, supplier.ContactPhone, supplier.Address, supplier.City, supplier.PostalCode, supplier.UpdatedAt);
        }
    }

    public class EndPoint : IEndpointDefinition
    {
        public void MapEndpoints(WebApplication app)
        {
            app.MapPut("api/supplier/{id}", async (ISender sender, int id, UpdateSupplier.Command command, CancellationToken cancellationToken) =>
            {
                if (id != command.Id)
                {
                    return Results.BadRequest("ID in the URL does not match ID in the request body.");
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


