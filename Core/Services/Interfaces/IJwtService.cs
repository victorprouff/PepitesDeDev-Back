namespace Core.Services.Interfaces;

public interface IJwtService
{
    public string GenerateJwtToken(Guid userId, bool isAdmin);
    public Guid? ValidateJwtToken(string? token);
    public Guid GetUserId(string? token);
}