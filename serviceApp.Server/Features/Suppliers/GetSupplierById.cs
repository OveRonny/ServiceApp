namespace serviceApp.Server.Features.Suppliers;

public static class GetSupplierById
{
    public record Query(int Id) : IQuery<Response>;
    public record Response(int Id, string Name, string ContactEmail, string ContactPhone, string Address, string City, string PostalCode);
    public class Handler(ApplicationDbContext context) : IQueryHandler<Query, Response>
    {
        private readonly ApplicationDbContext context = context;
        public async Task<Result<Response>> Handle(Query request, CancellationToken cancellationToken)
        {
            var supplier = await context.Suppliers
                .Where(s => s.Id == request.Id)
                .Select(s => new Response(s.Id, s.Name, s.ContactEmail, s.ContactPhone, s.Address, s.City, s.PostalCode))
                .FirstOrDefaultAsync(cancellationToken);
            if (supplier == null)
            {
                return Result.Fail<Response>($"Supplier with ID {request.Id} not found.");
            }
            return supplier;
        }
    }
}

[ApiController]
[Route("api/supplier")]
public class GetSupplierByIdController(ISender sender) : ControllerBase
{
    private readonly ISender sender = sender;

    [HttpGet("{id}")]
    public async Task<ActionResult<GetSupplierById.Query>> GetSupplierById(int id)
    {
        var result = await sender.Send(new GetSupplierById.Query(id));
        if (result.Failure)
        {
            return NotFound(result.Error);
        }
        return Ok(result.Value);
    }
}

