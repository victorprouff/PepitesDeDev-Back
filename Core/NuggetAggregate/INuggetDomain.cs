using Core.NuggetAggregate.Models;

namespace Core.NuggetAggregate;

public interface INuggetDomain
{
    Task<Guid> CreateAsync(CreateNuggetCommand createNuggetCommand);
    Task UpdateAsync(UpdateNuggetCommand createNuggetCommand);
    Task DeleteAsync(Guid id, Guid userId);
    Task<Nugget?> GetAsync(Guid id);
    Task<GetAllResponse> GetAllAsync(int limit, int offset);
    Task<GetAllResponse> GetAllByUserIdAsync(Guid getUserId, int limit, int offset);
}