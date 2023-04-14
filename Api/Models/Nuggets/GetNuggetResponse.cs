using Core.NuggetAggregate;
using NodaTime;

namespace Api.Models.Nuggets;

public record GetNuggetResponse(Guid Id, string Title, string Description, Instant CreatedAt, Instant? UpdatedAt)
{
    public static explicit operator GetNuggetResponse(Nugget nugget) =>
        new(nugget.Id, nugget.Title, nugget.Description, nugget.CreatedAt, nugget.UpdatedAt);
}