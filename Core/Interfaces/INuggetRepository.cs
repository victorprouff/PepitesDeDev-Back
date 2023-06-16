using Core.NuggetAggregate;

namespace Core.Interfaces;

public interface INuggetRepository
{
    Task CreateAsync(Nugget nugget, CancellationToken cancellationToken);
    Task UpdateAsync(Nugget nugget, CancellationToken cancellationToken);
    Task Delete(Guid id, CancellationToken cancellationToken);
    Task<(int, IEnumerable<Nugget>)> GetAll(int limit, int offset, CancellationToken cancellationToken);
    Task<(int, IEnumerable<Nugget>)> GetAllByUserId(Guid userId, bool isAdmin, int limit, int offset, CancellationToken cancellationToken);
    Task<Nugget?> GetById(Guid id, CancellationToken cancellationToken);
}