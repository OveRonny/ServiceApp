using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using serviceApp.Server.Features.Autentication;
using System.Data;

namespace serviceApp.Server.Features.Users;

public static class CreateNewUser
{
    public record Command(string UserName, string Email, string Password, string PhoneNumber, Guid? FamilyId, string[] Roles) : ICommand<Response>;

    public record Response(string UserName, string Email, string PhoneNumber);

    public class Handler(UserManager<ApplicationUser> userManager, ICurrentUser currentUser, RoleManager<IdentityRole> roleManager) : ICommandHandler<Command, Response>
    {
        private readonly UserManager<ApplicationUser> _users = userManager;
        private readonly ICurrentUser _current = currentUser;
        private readonly RoleManager<IdentityRole> _roles = roleManager;

        public async Task<Result<Response>> Handle(Command request, CancellationToken cancellationToken)
        {
            var existing = await _users.FindByEmailAsync(request.Email);
            if (existing is not null)
                return Result.Fail<Response>("Email is already in use.");

            var user = new ApplicationUser
            {
                UserName = request.UserName,
                Email = request.Email,
                PhoneNumber = request.PhoneNumber,
                EmailConfirmed = true,
                FamilyId =  Guid.NewGuid()
            };

            await _users.CreateAsync(user, request.Password);

            var roleResult = await EnsureRolesAndAssignAsync(user, request.Roles);
            if (roleResult.Failure)
                return Result.Fail<Response>(roleResult.Error!);

            return Result.Ok(new Response(user.UserName!, user.Email!, user.PhoneNumber ?? string.Empty));
        }

        private async Task<Result> EnsureRolesAndAssignAsync(ApplicationUser user, IEnumerable<string>? roles)
        {
            var normalized = (roles ?? Array.Empty<string>())
                .Where(r => !string.IsNullOrWhiteSpace(r))
                .Select(r => r.Trim())
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToArray();

            if (normalized.Length == 0)
                return Result.Ok();

            foreach (var role in normalized)
            {
                if (!await _roles.RoleExistsAsync(role))
                    return Result.Fail($"Role '{role}' does not exist.");
            }

            var add = await _users.AddToRolesAsync(user, normalized);
            if (!add.Succeeded)
            {
                var err = string.Join("; ", add.Errors.Select(e => e.Description));
                return Result.Fail($"Failed assigning roles: {err}");
            }

            return Result.Ok();
        }
    }
    public class EndPoint : IEndpointDefinition
    {
        public void MapEndpoints(WebApplication app)
        {
            app.MapPost("api/user", async (ISender sender, Command command, CancellationToken cancellationToken) =>
            {
                var result = await sender.Send(command, cancellationToken);
                return result.Failure ? Results.BadRequest(result.Error) : Results.Ok(result.Value);
            })
            .RequireAuthorization(new AuthorizeAttribute { Roles = "OwnerAdmin" });
        }
    }
}



