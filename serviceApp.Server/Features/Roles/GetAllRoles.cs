using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace serviceApp.Server.Features.Roles;

public static class GetAllRoles
{
    public record Query() : IQuery<IEnumerable<string>>;
    public class Handler(RoleManager<IdentityRole> roleManager) : IQueryHandler<Query, IEnumerable<string>>
    {
        private readonly RoleManager<IdentityRole> _roles = roleManager;
        public async Task<Result<IEnumerable<string>>> Handle(Query request, CancellationToken cancellationToken)
        {
            var roles = await _roles.Roles.Select(r => r.Name).ToListAsync(cancellationToken);
            return Result.Ok((IEnumerable<string>)roles);
        }
    }

    public class EndPoint : IEndpointDefinition
    {
        public void MapEndpoints(WebApplication app)
        {
            app.MapGet("api/roles", async (ISender sender, CancellationToken cancellationToken) =>
            {
                var result = await sender.Send(new Query(), cancellationToken);
                return result.Failure ? Results.Unauthorized() : Results.Ok(result.Value);
            })
            .RequireAuthorization(new AuthorizeAttribute { Roles = "OwnerAdmin" });
        }
    }
}
