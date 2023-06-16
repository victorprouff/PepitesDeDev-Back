using NodaTime;

namespace Api.Models.Nuggets;

public record GetAllNuggetResponse(int NbOfNuggets, IEnumerable<Nugget> Nuggets);

public record Nugget(Guid Id, string Title, string Content, string Creator, Guid UserId, Instant CreatedAt, Instant? UpdatedAt)
{
    public static explicit operator Nugget(Core.NuggetAggregate.Projections.Nugget nugget) =>
        new(nugget.Id, nugget.Title, nugget.Content, nugget.Creator, nugget.UserId, nugget.CreatedAt, nugget.UpdatedAt);
}