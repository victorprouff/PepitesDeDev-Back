namespace Core.UserAggregate.Models;

public record AuthenticateResponse(Guid Id, string Email, string Token);