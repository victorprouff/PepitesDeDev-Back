using NodaTime;

namespace Core.NuggetAggregate.Projections;

public record GetAllNuggetsProjection(int NbOfNuggets, IEnumerable<Nugget> Nuggets);

public record Nugget(Guid Id, string Title, string Content, string Creator, Guid UserId, Instant CreatedAt, Instant? UpdatedAt);