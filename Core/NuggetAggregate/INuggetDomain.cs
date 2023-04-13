using Core.NuggetAggregate.Models;

namespace Core.NuggetAggregate;

public interface INuggetDomain
{
    void Create(CreateNuggetCommand createNuggetCommand);
    void Update(UpdateNuggetCommand createNuggetCommand);
    Nugget? Get(Guid id);
    IEnumerable<Nugget> Get();

    void Delete(Guid id);
}