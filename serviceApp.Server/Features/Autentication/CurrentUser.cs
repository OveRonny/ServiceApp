using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace serviceApp.Server.Features.Autentication;

public sealed class CurrentUser(
    IHttpContextAccessor accessor,
    UserManager<ApplicationUser> userManager) : ICurrentUser
{
    private readonly IHttpContextAccessor _accessor = accessor;
    private readonly UserManager<ApplicationUser> _userManager = userManager;
    private Guid? _familyId; // cache per request

    public bool IsAuthenticated => _accessor.HttpContext?.User?.Identity?.IsAuthenticated ?? false;
    public string? UserId => _accessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);

    public Guid? FamilyId
    {
        get
        {
            if (_familyId.HasValue) return _familyId;
            var userId = UserId;
            if (!IsAuthenticated || string.IsNullOrEmpty(userId)) return null;

            // Synchronous access for simplicity (per-request). If you prefer async, convert to async flow.
            var user = _userManager.FindByIdAsync(userId).GetAwaiter().GetResult();
            _familyId = user?.FamilyId;
            return _familyId;
        }
    }
}
