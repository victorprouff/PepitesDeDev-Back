namespace Core.UserAggregate.Models;

public record CreateUserCommand(Email Email, string username, string Password);