using Core.NuggetAggregate;
using Microsoft.AspNetCore.Components;
using Nugget = Application.Models.Nugget;

namespace Application.Services;

public class NuggetService : INuggetService
{
    [Inject] IServiceProvider Sp { get; set; }

    private readonly INuggetDomain _nuggetDomain;

    public NuggetService()
    {
        _nuggetDomain = Sp.GetRequiredService<INuggetDomain>();

    }
    public async Task<IEnumerable<Nugget>> GetAll(CancellationToken cancellationToken)
    {
        try
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
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}