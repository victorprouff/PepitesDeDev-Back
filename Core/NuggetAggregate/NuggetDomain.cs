using Core.Interfaces;
using Core.NuggetAggregate.Models;
using NodaTime;

namespace Core.NuggetAggregate;

public class NuggetDomain : INuggetDomain
{
    private readonly IClock _clock;
    private readonly INuggetRepository _repository;
    
    public NuggetDomain(IClock clock, INuggetRepository repository)
    {
        _clock = clock;
        _repository = repository;
    }

    public async Task Create(CreateNuggetCommand createNuggetCommand)
    {
        await _repository.CreateAsync(
            Nugget.Create(createNuggetCommand.Title,
                createNuggetCommand.Description,
                _clock.GetCurrentInstant()));
    }

    public async Task Update(UpdateNuggetCommand createNuggetCommand)
    {
        var nugget = await _repository.GetById(createNuggetCommand.Id);
        if (nugget is null)
        {
            throw new Exception(); // Todo : implémenter les exceptions
        }
        
        nugget.Update(createNuggetCommand.Title, createNuggetCommand.Description, _clock.GetCurrentInstant());
        
        await _repository.UpdateAsync(nugget);
    }

    public async Task<Nugget?> Get(Guid id) => await _repository.GetById(id);

    public async Task<IEnumerable<Nugget>> Get() => await _repository.Get();
    
    public void Delete(Guid id)
    {
        _repository.Delete(id);
    }
}