using Core.NuggetAggregate;

namespace Core.Interfaces;

public interface INuggetRepository
{
    Task CreateAsync(Nugget nugget, CancellationToken cancellationToken = default);
    Task UpdateAsync(Nugget nugget, CancellationToken cancellationToken = default);
    Task Delete(Guid id, CancellationToken cancellationToken = default);
    Task<(int, IEnumerable<Nugget>)> GetAll(int limit, int offset, CancellationToken cancellationToken = default);
    Task<(int, IEnumerable<Nugget>)> GetAllByUserId(Guid userId, int limit, int offset, CancellationToken cancellationToken = default);
    Task<Nugget?> GetById(Guid id, CancellationToken cancellationToken = default);
}