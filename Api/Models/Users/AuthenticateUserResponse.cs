using Core.UserAggregate.Models;

namespace Api.Models.Users;

public record AuthenticateUserResponse(Guid Id, string Email, string Token)
{
    public static explicit operator AuthenticateUserResponse(AuthenticateResponse model) =>
        new AuthenticateUserResponse(model.Id, model.Email, model.Token);
}