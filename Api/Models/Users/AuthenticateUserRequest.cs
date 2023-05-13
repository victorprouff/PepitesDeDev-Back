namespace Api.Models.Users;

public record AuthenticateUserRequest(string Email, string Password);