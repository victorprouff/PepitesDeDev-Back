using Core.NuggetAggregate.Models;

namespace Core.NuggetAggregate;

public interface INuggetDomain
{
    Task Create(CreateNuggetCommand createNuggetCommand);
    Task Update(UpdateNuggetCommand createNuggetCommand);
    Task<Nugget?> Get(Guid id);
    Task<IEnumerable<Nugget>> Get();
    void Delete(Guid id);
}