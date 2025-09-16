namespace serviceApp.Server.Features.Autentication;

public interface ICurrentUser
{
    bool IsAuthenticated { get; }
    string? UserId { get; }
    Guid? FamilyId { get; }
}