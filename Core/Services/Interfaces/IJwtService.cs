namespace Core.Services.Interfaces;

public interface IJwtService
{
    public string GenerateJwtToken(Guid userId);
    public Guid? ValidateJwtToken(string? token);
    public Guid GetUserId(string? token);
}