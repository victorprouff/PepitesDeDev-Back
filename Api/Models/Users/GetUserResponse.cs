using Core.UserAggregate.Models;
using NodaTime;

namespace Api.Models.Users;

public record GetUserResponse(Guid Id, string Email, string Username, bool IsAdmin, Instant CreatedAt, Instant? UpdatedAt)
{
    public static explicit operator GetUserResponse(GetUserByIdQueryResponse user) =>
        new(user.Id, user.Email, user.Username, user.IsAdmin, user.CreatedAt, user.UpdatedAt);
}