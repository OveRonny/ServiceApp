namespace serviceApp.Server.Features.Suppliers;

public static class GetAllSuppliers
{
    public record Query() : IQuery<Response>;
    public record Response(List<SupplierDto> Suppliers);
    public record SupplierDto(int Id, string Name, string ContactEmail, string ContactPhone, string Address, string City, string PostalCode);
    public class Handler(ApplicationDbContext context) : IQueryHandler<Query, Response>
    {
        private readonly ApplicationDbContext context = context;
        public async Task<Result<Response>> Handle(Query request, CancellationToken cancellationToken)
        {
            var suppliers = await context.Suppliers
                .Select(s => new SupplierDto(s.Id, s.Name, s.ContactEmail, s.ContactPhone, s.Address, s.City, s.PostalCode))
                .ToListAsync(cancellationToken);
            return new Response(suppliers);
        }
    }
}

[ApiController]
[Route("api/supplier")]
public class GetAllSuppliersController(ISender sender) : ControllerBase
{
    private readonly ISender sender = sender;

    [HttpGet]
    public async Task<ActionResult<GetAllSuppliers.Response>> GetAllSuppliers()
    {
        var result = await sender.Send(new GetAllSuppliers.Query());
        return Ok(result);
    }
}
