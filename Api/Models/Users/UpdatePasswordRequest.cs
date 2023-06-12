namespace Api.Models.Users;

public record UpdatePasswordRequest(string OldPassword, string NewPassword);