namespace Api.Models.Users;

public record AuthenticateUserRequest(string EmailOrUsername, string Password);