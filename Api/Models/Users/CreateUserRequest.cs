namespace Api.Models.Users;

public record CreateUserRequest(string Email, string Username, string Password);