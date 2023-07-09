using Core.Exceptions;
using Core.Interfaces;
using Core.NuggetAggregate.Exceptions;
using Core.NuggetAggregate.Models;
using Core.NuggetAggregate.Projections;
using NodaTime;

namespace Core.NuggetAggregate;

public class NuggetDomain : INuggetDomain
{
    private readonly IClock _clock;
    private readonly INuggetRepository _repository;
    private readonly IUserRepository _userRepository;

    public NuggetDomain(IClock clock, INuggetRepository repository, IUserRepository userRepository)
    {
        _clock = clock;
        _repository = repository;
        _userRepository = userRepository;
    }

    public async Task<Guid> CreateAsync(CreateNuggetCommand createNuggetCommand, CancellationToken cancellationToken)
    {
        var newNugget = Nugget.Create(
            createNuggetCommand.Title,
            createNuggetCommand.Content,
            createNuggetCommand.UserId,
            _clock.GetCurrentInstant());
        
        await _repository.CreateAsync(newNugget, cancellationToken);

        return newNugget.Id;
    }

    public async Task UpdateAsync(UpdateNuggetCommand updateNuggetCommand, CancellationToken cancellationToken)
    {
        var nugget = await _repository.GetById(updateNuggetCommand.Id, cancellationToken)
            ?? throw new NotFoundException($"The nugget with id {updateNuggetCommand.Id} is not found.");

        var userIsAdmin = await _userRepository.CheckIfIsAdmin(updateNuggetCommand.UserId, cancellationToken);
        if (nugget.UserId != updateNuggetCommand.UserId && userIsAdmin is false)
        {
            throw new NuggetDoesNotBelongToUserException(
                $"The nugget with id {updateNuggetCommand.Id} doesn't belong to the user with id {updateNuggetCommand.UserId}.");
        }
        
        nugget.Update(updateNuggetCommand.Title, updateNuggetCommand.Content, _clock.GetCurrentInstant());
        
        await _repository.UpdateAsync(nugget, cancellationToken);
    }

    public async Task<GetNuggetProjection> GetAsync(Guid id, CancellationToken cancellationToken) =>
        await _repository.GetByIdProjection(id, cancellationToken)
        ?? throw new NotFoundException($"The nugget with id {id} is not found.");

    public async Task<GetAllNuggetsProjection> GetAllAsync(int limit, int offset, CancellationToken cancellationToken) =>
        await _repository.GetAll(limit, offset, cancellationToken);

    public async Task<GetAllNuggetsProjection> GetAllByUserIdOrAdminAsync(
        Guid userId,
        int limit,
        int offset,
        CancellationToken cancellationToken) =>
        await _repository.GetAllByUserIdProjection(
            userId,
            limit,
            offset,
            cancellationToken);

    public async Task DeleteAsync(Guid id, Guid userId, CancellationToken cancellationToken)
    {
        var nugget = await _repository.GetById(id, cancellationToken)
            ?? throw new NotFoundException($"The nugget with id {id} is not found.");
        
        var userIsAdmin = await _userRepository.CheckIfIsAdmin(userId, cancellationToken);
        if (nugget.UserId != userId && userIsAdmin is false)
        {
            throw new NuggetDoesNotBelongToUserException($"The nugget with id {id} doesn't belong to the user with id {userId}.");
        }
        
        await _repository.Delete(id, cancellationToken);
    }
}