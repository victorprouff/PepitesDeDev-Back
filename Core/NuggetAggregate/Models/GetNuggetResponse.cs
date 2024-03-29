using Core.NuggetAggregate.Projections;
using NodaTime;

namespace Core.NuggetAggregate.Models;

public record GetNuggetResponse(Guid Id, string Title, string Content, bool IsEnabled, string? UrlImage, string Creator, Guid UserId, Instant CreatedAt, Instant? UpdatedAt)
{
    public static explicit operator GetNuggetResponse(GetNuggetProjection nugget) =>
        new(nugget.Id, nugget.Title, nugget.Content, nugget.IsEnabled, nugget.UrlImage, nugget.Creator, nugget.UserId, nugget.CreatedAt, nugget.UpdatedAt);
}