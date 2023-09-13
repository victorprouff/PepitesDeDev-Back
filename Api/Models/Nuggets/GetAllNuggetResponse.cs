using NodaTime;

namespace Api.Models.Nuggets;

public record GetAllNuggetResponse(int NbOfNuggets, IEnumerable<Nugget> Nuggets);

public record Nugget(Guid Id, string Title, string Content, bool IsEnabled, string? UrlImage, string Creator, Guid UserId, Instant CreatedAt, Instant? UpdatedAt)
{
    public static explicit operator Nugget(Core.NuggetAggregate.Projections.Nugget nugget) =>
        new(nugget.Id, nugget.Title, nugget.Content, nugget.IsEnabled, nugget.UrlImage, nugget.Creator, nugget.UserId, nugget.CreatedAt, nugget.UpdatedAt);
}