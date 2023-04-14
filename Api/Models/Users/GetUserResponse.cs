using Core.UserAggregate.Models;
using NodaTime;

namespace Api.Models.Users;

public record GetUserResponse(Guid Id, string Email, Instant CreatedAt, Instant? UpdatedAt)
{
    public static explicit operator GetUserResponse(GetUserByIdQueryResponse user) =>
        new(user.Id, user.Email, user.CreatedAt, user.UpdatedAt);
}