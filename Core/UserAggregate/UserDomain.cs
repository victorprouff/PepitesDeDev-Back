using Core.Interfaces;
using Core.Services.Interfaces;
using Core.UserAggregate.Models;
using NodaTime;

namespace Core.UserAggregate;

public class UserDomain : IUserDomain
{
    private readonly IClock _clock;
    private readonly IUserRepository _repository;
    private readonly IJwtService _jwtService;
    private readonly IPasswordEncryptor _passwordEncryptor;

    public UserDomain(IClock clock, IUserRepository repository, IJwtService jwtService, IPasswordEncryptor passwordEncryptor)
    {
        _clock = clock;
        _repository = repository;
        _jwtService = jwtService;
        _passwordEncryptor = passwordEncryptor;
    }

    public async Task<AuthenticateResponse?> Authenticate(string emailOrUsername, string password)
    {
        var user = await _repository.GetByEmailOrUsernameAsync(emailOrUsername);
        if (user is null)
        {
            return null; // Todo: throw exception user notfound ?
        }

        if (_passwordEncryptor.VerifyPassword(password, user.Salt, user.Password) is false)
        {
            throw new Exception(); // todo: exception password incorrect
        }
        
        var token = _jwtService.GenerateJwtToken(user.Id);

        return new AuthenticateResponse(user.Id, user.Email.Value, user.Username, token);
    }

    public async Task<Guid> CreateAsync(CreateUserCommand user, CancellationToken cancellationToken = default)
    {
        var salt = _passwordEncryptor.GenerateSalt();
        var passwordHash = _passwordEncryptor.GenerateHash(user.Password, salt);

        var newUser = User.Create(user.Email, user.username, passwordHash, salt, _clock.GetCurrentInstant());
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