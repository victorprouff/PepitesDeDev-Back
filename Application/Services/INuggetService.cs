using Application.Models;

namespace Application.Services;

public interface INuggetService
{
    public Task<IEnumerable<Nugget>> GetAll(CancellationToken cancellationToken);
}