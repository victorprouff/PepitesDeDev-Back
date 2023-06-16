using NodaTime;

namespace Core.UserAggregate.Models;

public record GetUserByIdQueryResponse(Guid Id, string Email, string Username, bool IsAdmin, Instant CreatedAt, Instant? UpdatedAt)
{
    public static explicit operator GetUserByIdQueryResponse?(User? user) =>
        user is null
            ? null
            : new GetUserByIdQueryResponse(user.Id, user.Email.Value, user.Username, user.IsAdmin, user.CreatedAt, user.UpdatedAt);
}