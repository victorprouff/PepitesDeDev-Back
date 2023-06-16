using Core.UserAggregate.Models;

namespace Api.Models.Users;

public record AuthenticateUserResponse(Guid Id, string Email, string Username, bool IsAdmin, string Token)
{
    public static explicit operator AuthenticateUserResponse(AuthenticateResponse model) =>
        new(model.Id, model.Email, model.Username, model.IsAdmin, model.Token);
}