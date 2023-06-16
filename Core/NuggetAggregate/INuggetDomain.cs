using Core.NuggetAggregate.Models;

namespace Core.NuggetAggregate;

public interface INuggetDomain
{
    Task<Guid> CreateAsync(CreateNuggetCommand createNuggetCommand, CancellationToken cancellationToken);
    Task UpdateAsync(UpdateNuggetCommand createNuggetCommand, CancellationToken cancellationToken);
    Task DeleteAsync(Guid id, Guid userId, CancellationToken cancellationToken);
    Task<Nugget?> GetAsync(Guid id, CancellationToken cancellationToken);
    Task<GetAllResponse> GetAllAsync(int limit, int offset, CancellationToken cancellationToken);
    Task<GetAllResponse> GetAllByUserIdOrAdminAsync(
        Guid getUserId,
        int limit,
        int offset,
        CancellationToken cancellationToken);
}