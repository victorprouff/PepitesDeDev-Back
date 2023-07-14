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
    private readonly IFileStorage _fileStorage;
    private readonly string _cleverCloudHost;

    private const string BucketName = "nuggets-images";
    
    public NuggetDomain(
        IClock clock,
        INuggetRepository repository,
        IUserRepository userRepository,
        IFileStorage fileStorage,
        string cleverCloudHost)
    {
        _clock = clock;
        _repository = repository;
        _userRepository = userRepository;
        _fileStorage = fileStorage;
        _cleverCloudHost = cleverCloudHost;
    }

    public async Task<Guid> CreateAsync(CreateNuggetCommand createNuggetCommand, CancellationToken cancellationToken)
    {
        var fileName = createNuggetCommand.FileNameImage ?? Guid.NewGuid().ToString();
        var fullPath = Path.Combine($"https://{BucketName}.{_cleverCloudHost}/", fileName);
        
        if (createNuggetCommand.Stream.Length > 0)
        {
            await _fileStorage.UploadFileAsync(BucketName, fileName, createNuggetCommand.Stream, cancellationToken);
        }
        else
        {
            fullPath = null;
        }
        
        var newNugget = Nugget.Create(
            createNuggetCommand.Title,
            createNuggetCommand.Content,
            fullPath,
            createNuggetCommand.UserId,
            _clock.GetCurrentInstant());

        try
        {
            await _repository.CreateAsync(newNugget, cancellationToken);
        }
        catch (Exception e)
        {
            if (string.IsNullOrWhiteSpace(fullPath) is false)
            {
                await _fileStorage.DeleteFileAsync(BucketName, Path.GetFileName(fullPath), cancellationToken);
            }
            
            throw new Exception("", e);
        }

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

    public async Task<string> UpdateImageAsync(UpdateNuggetImageCommand command, CancellationToken cancellationToken)
    {
        var nugget = await _repository.GetById(command.NuggetId, cancellationToken)
                     ?? throw new NotFoundException($"The nugget with id {command.NuggetId} is not found.");
        
        var userIsAdmin = await _userRepository.CheckIfIsAdmin(command.UserId, cancellationToken);
        if (nugget.UserId != command.UserId && userIsAdmin is false)
        {
            throw new NuggetDoesNotBelongToUserException(
                $"The nugget with id {command.NuggetId} doesn't belong to the user with id {command.UserId}.");
        }
        
        var fileName = command.FileName ?? Guid.NewGuid().ToString();
        var fullPath = Path.Combine($"https://{BucketName}.{_cleverCloudHost}/", fileName);

        await _fileStorage.UploadFileAsync(BucketName, fileName, command.Stream, cancellationToken);

        nugget.UpdateUrlImage(fullPath, _clock.GetCurrentInstant());

        await _repository.UpdateUrlImageAsync(nugget, cancellationToken);

        return fullPath;
    }

    public async Task<GetNuggetProjection> GetAsync(Guid id, CancellationToken cancellationToken) =>
        await _repository.GetByIdProjection(id, cancellationToken)
        ?? throw new NotFoundException($"The nugget with id {id} is not found.");

    public async Task<GetAllNuggetsProjection>
        GetAllAsync(int limit, int offset, CancellationToken cancellationToken) =>
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
            throw new NuggetDoesNotBelongToUserException(
                $"The nugget with id {id} doesn't belong to the user with id {userId}.");
        }

        if (nugget.UrlImage is not null)
        {
            await _fileStorage.DeleteFileAsync(BucketName, Path.GetFileName(nugget.UrlImage), cancellationToken);
        }
        
        await _repository.Delete(id, cancellationToken);
    }
}