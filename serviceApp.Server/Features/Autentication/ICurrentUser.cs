
namespace serviceApp.Server.Features.Autentication;

public interface ICurrentUser
{
    bool IsAuthenticated { get; }
    string? UserId { get; }
    Guid? FamilyIdClaim { get; }

    Task<Guid?> GetFamilyIdAsync(CancellationToken ct = default);
}