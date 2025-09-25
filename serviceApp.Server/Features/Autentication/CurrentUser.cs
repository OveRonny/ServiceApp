using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace serviceApp.Server.Features.Autentication;

public sealed class CurrentUser(
    IHttpContextAccessor accessor,
    UserManager<ApplicationUser> userManager) : ICurrentUser
{
    private readonly IHttpContextAccessor _accessor = accessor;
    private readonly UserManager<ApplicationUser> _userManager = userManager;
    private Guid? _familyIdCache; // cache per request

    public bool IsAuthenticated => _accessor.HttpContext?.User?.Identity?.IsAuthenticated ?? false;
    public string? UserId => _accessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);

    // If you add a "family_id" claim at sign-in, we can read it without a DB call
    public Guid? FamilyIdClaim
    {
        get
        {
            var val = _accessor.HttpContext?.User?.FindFirstValue("family_id");
            return Guid.TryParse(val, out var gid) ? gid : null;
        }
    }

    public async Task<Guid?> GetFamilyIdAsync(CancellationToken ct = default)
    {
        if (_familyIdCache.HasValue) return _familyIdCache;
        if (!IsAuthenticated || string.IsNullOrEmpty(UserId)) return null;

        // Prefer claim if present (no DB hit)
        var fromClaim = FamilyIdClaim;
        if (fromClaim.HasValue)
        {
            _familyIdCache = fromClaim;
            return _familyIdCache;
        }

        // Lightweight DB query: only fetch FamilyId, no tracking
        var uid = UserId!;
        var familyId = await _userManager.Users
            .AsNoTracking()
            .Where(u => u.Id == uid)
            .Select(u => (Guid?)u.FamilyId)
            .FirstOrDefaultAsync(ct);

        _familyIdCache = familyId;
        return _familyIdCache;
    }
}
