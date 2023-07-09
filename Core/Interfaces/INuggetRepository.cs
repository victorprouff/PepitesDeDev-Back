using Core.NuggetAggregate.Projections;
using Nugget = Core.NuggetAggregate.Nugget;

namespace Core.Interfaces;

public interface INuggetRepository
{
    Task CreateAsync(Nugget nugget, CancellationToken cancellationToken);
    Task UpdateAsync(Nugget nugget, CancellationToken cancellationToken);
    Task UpdateUrlImageAsync(Nugget nugget, CancellationToken cancellationToken);
    Task Delete(Guid id, CancellationToken cancellationToken);
    Task<GetAllNuggetsProjection> GetAll(int limit, int offset, CancellationToken cancellationToken);
    Task<GetAllNuggetsProjection> GetAllByUserIdProjection(Guid userId, int limit, int offset, CancellationToken cancellationToken);
    Task<GetNuggetProjection?> GetByIdProjection(Guid id, CancellationToken cancellationToken);
    Task<Nugget?> GetById(Guid id, CancellationToken cancellationToken);
}