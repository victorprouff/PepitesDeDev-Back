using Core.NuggetAggregate.Models;
using Core.NuggetAggregate.Projections;

namespace Core.NuggetAggregate;

public interface INuggetDomain
{
    Task<Guid> CreateAsync(CreateNuggetCommand createNuggetCommand, CancellationToken cancellationToken);
    Task UpdateAsync(UpdateNuggetCommand createNuggetCommand, CancellationToken cancellationToken);
    Task DeleteAsync(Guid id, Guid userId, CancellationToken cancellationToken);
    Task<GetNuggetProjection?> GetAsync(Guid id, CancellationToken cancellationToken);
    Task<GetAllNuggetsProjection> GetAllAsync(int limit, int offset, CancellationToken cancellationToken);
    Task<GetAllNuggetsProjection> GetAllByUserIdOrAdminAsync(
        Guid getUserId,
        int limit,
        int offset,
        CancellationToken cancellationToken);
}