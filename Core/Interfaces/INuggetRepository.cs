using Core.NuggetAggregate;

namespace Core.Interfaces;

public interface INuggetRepository
{
    Task CreateAsync(Nugget nugget, CancellationToken cancellationToken = default);
    Task UpdateAsync(Nugget nugget, CancellationToken cancellationToken = default);
    Task Delete(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<Nugget>> GetAll(CancellationToken cancellationToken = default);
    Task<Nugget?> GetById(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<Nugget>> GetAllByUserId(Guid userId, CancellationToken cancellationToken = default);
}