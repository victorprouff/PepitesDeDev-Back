namespace Core.UserAggregate.Models;

public record CreateUserCommand(string Email, string Password);