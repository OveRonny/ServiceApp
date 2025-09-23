namespace serviceApp.Server.Features.Suppliers;

public static class CreateSupplier
{
    public record Command(string Name, string ContactEmail, string ContactPhone, string Address, string City, string PostalCode) : ICommand<Response>;
    public record Response(int Id, string Name, string ContactEmail, string ContactPhone, string Address, string City, string PostalCode, DateTime CreatedAt);

    public class Handler(ApplicationDbContext context) : ICommandHandler<Command, Response>
    {
        private readonly ApplicationDbContext context = context;
        public async Task<Result<Response>> Handle(Command request, CancellationToken cancellationToken)
        {
            var supplier = new Supplier
            {
                Name = request.Name,
                ContactEmail = request.ContactEmail,
                ContactPhone = request.ContactPhone,
                Address = request.Address,
                City = request.City,
                PostalCode = request.PostalCode,
                CreatedAt = DateTime.Now
            };
            context.Suppliers.Add(supplier);
            await context.SaveChangesAsync(cancellationToken);
            return new Response(supplier.Id, supplier.Name, supplier.ContactEmail, supplier.ContactPhone, supplier.Address, supplier.City, supplier.PostalCode, supplier.CreatedAt);
        }
    }

    public class EndPoint : IEndpointDefinition
    {
        public void MapEndpoints(WebApplication app)
        {
            app.MapPost("api/supplier", async (ISender sender, CreateSupplier.Command command, CancellationToken cancellationToken) =>
            {
                var result = await sender.Send(command, cancellationToken);
                return Results.Ok(result.Value);
            }).RequireAuthorization(); ;
        }
    }
}

