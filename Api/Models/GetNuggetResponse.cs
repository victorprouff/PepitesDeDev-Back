using Core.NuggetAggregate;

namespace Api.Models;

public record GetNuggetResponse(Guid Id, string Title, string Description)
{
    public static explicit operator GetNuggetResponse(Nugget nugget) =>
        new(nugget.Id, nugget.Title, nugget.Description);
}