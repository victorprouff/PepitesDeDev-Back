using Core.Exceptions;
using Core.Interfaces;
using Core.NuggetAggregate.Exceptions;
using Core.NuggetAggregate.Models;
using Core.NuggetAggregate.Projections;
using Microsoft.Extensions.Logging;
using NodaTime;

namespace Core.NuggetAggregate;

public class NuggetDomain : INuggetDomain
{
    private readonly IClock _clock;
    private readonly ILogger<NuggetDomain> _logger;
    private readonly INuggetRepository _nuggetRepository;
    private readonly IUserRepository _userRepository;
    private readonly IFileStorage _fileStorage;
    private readonly string _cleverCloudHost;

    private const string BucketName = "nuggets-images";

    public NuggetDomain(
        IClock clock,
        ILogger<NuggetDomain> logger,
        INuggetRepository nuggetRepository,
        IUserRepository userRepository,
        IFileStorage fileStorage,
        string cleverCloudHost)
    {
        _clock = clock;
        _logger = logger;
        _nuggetRepository = nuggetRepository;
        _userRepository = userRepository;
        _fileStorage = fileStorage;
        _cleverCloudHost = cleverCloudHost;
    }

    public async Task<Guid> CreateAsync(CreateNuggetCommand command, CancellationToken cancellationToken)
    {
        var fullPath = await SaveImageNugget(command.Stream, command.FileNameImage, cancellationToken);

        var newNugget = Nugget.Create(
            command.Title,
            command.Content,
            command.IsEnabled,
            fullPath,
            command.UserId,
            _clock.GetCurrentInstant());

        try
        {
            await _nuggetRepository.CreateAsync(newNugget, cancellationToken);
        }
        catch (Exception e)
        {
            if (string.IsNullOrWhiteSpace(fullPath) is false)
            {
                await _fileStorage.DeleteFileAsync(BucketName, Path.GetFileName(fullPath), cancellationToken);
                _logger.LogWarning("The nugget could not be created. The image has been deleted");
            }

            throw new NotBeCreatedException("The nugget could not be created.", e);
        }

        return newNugget.Id;
    }

    public async Task UpdateAsync(UpdateNuggetCommand command, CancellationToken cancellationToken)
    {
        var nugget = await _nuggetRepository.GetById(command.Id, cancellationToken)
                     ?? throw new NotFoundException($"The nugget with id {command.Id} is not found.");

        var userIsAdmin = await _userRepository.CheckIfIsAdmin(command.UserId, cancellationToken);
        if (nugget.UserId != command.UserId && userIsAdmin is false)
        {
            throw new NuggetDoesNotBelongToUserException(
                $"The nugget with id {command.Id} doesn't belong to the user with id {command.UserId}.");
        }

        var fullPath = await SaveImageNugget(
            command.Stream,
            command.FileNameImage,
            cancellationToken);

        if (fullPath is not null && nugget.UrlImage is not null)
        {
            await _fileStorage.DeleteFileAsync(BucketName, Path.GetFileName(nugget.UrlImage), cancellationToken);
        }

        nugget.Update(command.Title, command.Content, fullPath, _clock.GetCurrentInstant());

        await _nuggetRepository.UpdateAsync(nugget, cancellationToken);
    }

    public async Task<string> UpdateImageAsync(UpdateNuggetImageCommand command, CancellationToken cancellationToken)
    {
        var nugget = await _nuggetRepository.GetById(command.NuggetId, cancellationToken)
                     ?? throw new NotFoundException($"The nugget with id {command.NuggetId} is not found.");

        var userIsAdmin = await _userRepository.CheckIfIsAdmin(command.UserId, cancellationToken);
        if (nugget.UserId != command.UserId && userIsAdmin is false)
        {
            throw new NuggetDoesNotBelongToUserException(
                $"The nugget with id {command.NuggetId} doesn't belong to the user with id {command.UserId}.");
        }

        var fullPath = await SaveImageNugget(
                           command.Stream,
                           command.FileName ?? Guid.NewGuid().ToString(),
                           cancellationToken)
                       ?? throw new NuggetImageIsEmptyException("The image cannot be empty.");

        if (nugget.UrlImage is not null)
        {
            await _fileStorage.DeleteFileAsync(BucketName, Path.GetFileName(nugget.UrlImage), cancellationToken);
        }

        nugget.UpdateUrlImage(fullPath, _clock.GetCurrentInstant());

        await _nuggetRepository.UpdateUrlImageAsync(nugget, cancellationToken);

        return fullPath;
    }

    public async Task<GetNuggetProjection> GetAsync(Guid id, CancellationToken cancellationToken) =>
        await _nuggetRepository.GetByIdProjection(id, cancellationToken)
        ?? throw new NotFoundException($"The nugget with id {id} is not found.");

    public async Task<GetAllNuggetsProjection>
        GetAllAsync(int limit, int offset, CancellationToken cancellationToken) =>
        await _nuggetRepository.GetAll(limit, offset, cancellationToken);

    public async Task<GetAllNuggetsProjection> GetAllByUserIdOrAdminAsync(
        Guid userId,
        int limit,
        int offset,
        CancellationToken cancellationToken) =>
        await _nuggetRepository.GetAllByUserIdProjection(
            userId,
            limit,
            offset,
            cancellationToken);

    public async Task DeleteImageAsync(DeleteNuggetImageCommand command, CancellationToken cancellationToken)
    {
        var nugget = await _nuggetRepository.GetById(command.NuggetId, cancellationToken)
                     ?? throw new NotFoundException($"The nugget with id {command.NuggetId} is not found.");

        var userIsAdmin = await _userRepository.CheckIfIsAdmin(command.UserId, cancellationToken);
        if (nugget.UserId != command.UserId && userIsAdmin is false)
        {
            throw new NuggetDoesNotBelongToUserException(
                $"The nugget with id {command.NuggetId} doesn't belong to the user with id {command.UserId}.");
        }

        if (nugget.UrlImage is not null)
        {
            await _fileStorage.DeleteFileAsync(BucketName, Path.GetFileName(nugget.UrlImage), cancellationToken);
            nugget.UpdateUrlImage(null, _clock.GetCurrentInstant());

            await _nuggetRepository.UpdateUrlImageAsync(nugget, cancellationToken);
        }
    }

    public async Task DeleteAsync(Guid id, Guid userId, CancellationToken cancellationToken)
    {
        var nugget = await _nuggetRepository.GetById(id, cancellationToken)
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

        await _nuggetRepository.Delete(id, cancellationToken);
    }

    private async Task<string?> SaveImageNugget(
        MemoryStream stream,
        string? fileNameImage,
        CancellationToken cancellationToken)
    {
        if (stream.Length <= 0)
        {
            return null;
        }

        var fileName = $"{_clock.GetCurrentInstant().ToString()}-{fileNameImage ?? Guid.NewGuid().ToString()}";

        await _fileStorage.UploadFileAsync(BucketName, fileName, stream, cancellationToken);
        return GeneratePathFile(fileName);
    }

    private string GeneratePathFile(string fileName) =>
        Path.Combine($"https://{BucketName}.{_cleverCloudHost}/", fileName);
}