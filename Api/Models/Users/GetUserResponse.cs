using Core.UserAggregate.Models;
using NodaTime;

namespace Api.Models.Users;

public record GetUserResponse(Guid Id, string Email, string Username, Instant CreatedAt, Instant? UpdatedAt)
{
    public static explicit operator GetUserResponse(GetUserByIdQueryResponse user) =>
        new(user.Id, user.Email, user.Username, user.CreatedAt, user.UpdatedAt);
}