using Core.NuggetAggregate;
using NodaTime;

namespace Api.Models.Nuggets;

public record GetNuggetResponse(Guid Id, string Title, string Content, Guid UserId, Instant CreatedAt, Instant? UpdatedAt)
{
    public static explicit operator GetNuggetResponse(Nugget nugget) =>
        new(nugget.Id, nugget.Title, nugget.Content, nugget.UserId, nugget.CreatedAt, nugget.UpdatedAt);
}