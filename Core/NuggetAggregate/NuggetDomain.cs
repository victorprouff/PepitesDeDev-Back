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

    public async Task<Guid> CreateAsync(CreateNuggetCommand createNuggetCommand)
    {
        var newNugget = Nugget.Create(
            createNuggetCommand.Title,
            createNuggetCommand.Content,
            _clock.GetCurrentInstant());
        
        await _repository.CreateAsync(newNugget);

        return newNugget.Id;
    }

    public async Task UpdateAsync(UpdateNuggetCommand createNuggetCommand)
    {
        var nugget = await _repository.GetById(createNuggetCommand.Id);
        if (nugget is null)
        {
            throw new Exception(); // Todo : implémenter les exceptions
        }
        
        nugget.Update(createNuggetCommand.Title, createNuggetCommand.Content, _clock.GetCurrentInstant());
        
        await _repository.UpdateAsync(nugget);
    }

    public async Task<Nugget?> GetAsync(Guid id) => await _repository.GetById(id);

    public async Task<IEnumerable<Nugget>> GetAsync() => await _repository.Get();
    
    public void DeleteAsync(Guid id)
    {
        _repository.Delete(id);
    }
}