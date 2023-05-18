using Core.NuggetAggregate.Models;

namespace Core.NuggetAggregate;

public interface INuggetDomain
{
    Task<Guid> CreateAsync(CreateNuggetCommand createNuggetCommand);
    Task UpdateAsync(UpdateNuggetCommand createNuggetCommand);
    Task<Nugget?> GetAsync(Guid id);
    Task<IEnumerable<Nugget>> GetAsync();
    Task DeleteAsync(Guid id, Guid userId);
}