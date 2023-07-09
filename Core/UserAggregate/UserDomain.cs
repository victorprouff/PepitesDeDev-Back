using Core.Exceptions;
using Core.Interfaces;
using Core.Services.Interfaces;
using Core.UserAggregate.Exceptions;
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

    public async Task<AuthenticateResponse> Authenticate(string emailOrUsername, string password,
        CancellationToken cancellationToken = default)
    {
        var user = await _repository.GetByEmailOrUsernameAsync(emailOrUsername, cancellationToken);
        if (user is null)
        {
            throw new NotFoundException($"The user with email or username {emailOrUsername} was not found.");
        }

        if (_passwordEncryptor.VerifyPassword(password, user.Salt, user.Password) is false)
        {
            throw new BadPasswordException("The password is not correct");
        }
        
        var token = _jwtService.GenerateJwtToken(user.Id, user.IsAdmin);

        return new AuthenticateResponse(user.Id, user.Email.Value, user.Username, user.IsAdmin, token);
    }

    public async Task<Guid> CreateAsync(CreateUserCommand user, CancellationToken cancellationToken = default)
    {
        var salt = _passwordEncryptor.GenerateSalt();
        var passwordHash = _passwordEncryptor.GenerateHash(user.Password, salt);

        var newUser = User.Create(user.Email, user.username, passwordHash, salt, false,_clock.GetCurrentInstant());
        await _repository.CreateAsync(newUser, cancellationToken);

        return newUser.Id;
    }
    
    public Task UpdateEmail(Guid userId, Email newEmail, CancellationToken cancellationToken = default) =>
        _repository.UpdateEmail(userId, newEmail.Value, cancellationToken);

    public Task UpdateUsername(Guid userId, string username, CancellationToken cancellationToken = default) =>
        _repository.UpdateUsername(userId, username, cancellationToken);

    public async Task UpdatePassword(
        Guid userId,
        string oldPassword,
        string newPassword,
        CancellationToken cancellationToken)
    {
        var user = await _repository.GetByIdAsync(userId, cancellationToken);
        if (user is null)
        {
            throw new NotFoundException($"The user with id {userId} was not found.");
        }

        if (oldPassword == newPassword)
        {
            throw new BadPasswordException("The old password and the new password are identical. Please choose a different password.");
        }
        
        if (_passwordEncryptor.VerifyPassword(oldPassword, user.Salt, user.Password) is false)
        {
            throw new BadPasswordException("The password is not correct");
        }
        
        var salt = _passwordEncryptor.GenerateSalt();
        var passwordHash = _passwordEncryptor.GenerateHash(newPassword, salt);

        await _repository.UpdatePassword(userId, salt, passwordHash, cancellationToken);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        await _repository.DeleteAsync(id, cancellationToken);
    }

    public async Task<GetUserByIdQueryResponse> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var user = await _repository.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException($"The user with id {id} was not found.");
        
        return (GetUserByIdQueryResponse)user;
    }
}