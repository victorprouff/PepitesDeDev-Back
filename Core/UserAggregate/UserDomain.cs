using Core.Interfaces;
using Core.Services;
using Core.UserAggregate.Models;
using NodaTime;

namespace Core.UserAggregate;

public class UserDomain : IUserDomain
{
    private readonly IClock _clock;
    private readonly IUserRepository _repository;
    private readonly IJwtService _jwtService;

    public UserDomain(IClock clock, IUserRepository repository, IJwtService jwtService)
    {
        _clock = clock;
        _repository = repository;
        _jwtService = jwtService;
    }

    public async Task<string?> Authenticate(string email, string password)
    {
        var userId = await _repository.Authenticate(email, password);
        return userId is null ? null : _jwtService.GenerateJwtToken((Guid)userId);
    }

    public async Task<Guid> CreateAsync(CreateUserCommand user, CancellationToken cancellationToken = default)
    {
        var newUser = User.Create(user.Email, user.Password, _clock.GetCurrentInstant());
        await _repository.CreateAsync(newUser, cancellationToken);

        return newUser.Id;
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        await _repository.DeleteAsync(id, cancellationToken);
    }

    public async Task<GetUserByIdQueryResponse?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        (GetUserByIdQueryResponse?)await _repository.GetByIdAsync(id, cancellationToken);
}