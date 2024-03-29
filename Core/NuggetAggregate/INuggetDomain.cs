using Core.NuggetAggregate.Models;
using Core.NuggetAggregate.Projections;

namespace Core.NuggetAggregate;

public interface INuggetDomain
{
    Task<Guid> CreateAsync(CreateNuggetCommand command, CancellationToken cancellationToken);
    Task UpdateAsync(UpdateNuggetCommand command, CancellationToken cancellationToken);
    Task<string> UpdateImageAsync(UpdateNuggetImageCommand command, CancellationToken cancellationToken);
    Task DeleteAsync(Guid id, Guid userId, CancellationToken cancellationToken);
    Task<GetNuggetProjection> GetAsync(Guid id, CancellationToken cancellationToken);
    Task<GetAllNuggetsProjection> GetAllAsync(bool withDisabledNugget, int limit, int offset, CancellationToken cancellationToken);
    Task<GetAllNuggetsProjection> GetAllByUserIdOrAdminAsync(
        Guid getUserId,
        int limit,
        int offset,
        CancellationToken cancellationToken);

    Task DeleteImageAsync(DeleteNuggetImageCommand command, CancellationToken cancellationToken);
}