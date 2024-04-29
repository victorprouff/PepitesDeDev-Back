using Core.NuggetAggregate;
using Nugget = Application.Models.Nugget;

namespace Application.Services;

public class NuggetService : INuggetService
{
    private readonly INuggetDomain _nuggetDomain;

    public async Task<IEnumerable<Nugget>> GetAll(CancellationToken cancellationToken)
    {
        var response = await _nuggetDomain.GetAllAsync(true, 100, 0, cancellationToken);

        return response.Nuggets.Select(n => new Nugget(
            n.Id,
            n.Title,
            n.Content,
            n.IsEnabled,
            n.UrlImage,
            n.Creator,
            n.UserId,
            n.CreatedAt,
            n.UpdatedAt));
    }
}