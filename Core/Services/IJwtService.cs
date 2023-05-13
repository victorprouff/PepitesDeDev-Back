namespace Core.Services;

public interface IJwtService
{
    public string GenerateJwtToken(Guid userId);
    public Guid? ValidateJwtToken(string? token);
}