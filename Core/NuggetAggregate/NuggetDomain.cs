using Core.NuggetAggregate.Models;

namespace Core.NuggetAggregate;

public class NuggetDomain : INuggetDomain
{
    private List<Nugget> _nuggets = new();

    public void Create(CreateNuggetCommand createNuggetCommand)
    {
        _nuggets.Add(new Nugget(createNuggetCommand.Title, createNuggetCommand.Description));
    }

    public void Update(UpdateNuggetCommand createNuggetCommand)
    {
        var nugget = _nuggets.FirstOrDefault(n => n.Id == createNuggetCommand.Id);
        if (nugget is null)
        {
            throw new Exception(); // Todo : implémenter les exceptions
        }
        
        nugget.Update(createNuggetCommand.Title, createNuggetCommand.Description);
    }

    public Nugget? Get(Guid id) => _nuggets.FirstOrDefault(n => n.Id == id);

    public IEnumerable<Nugget> Get() => _nuggets;
    
    public void Delete(Guid id)
    {
        var nugget = _nuggets.FirstOrDefault(n => n.Id == id);
        if (nugget != null)
        {
            _nuggets.Remove(nugget);
        }
    }
}