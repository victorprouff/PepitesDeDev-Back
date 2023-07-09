using Core.UserAggregate.Models;

namespace Core.UserAggregate;

public interface IUserDomain
{
    Task<AuthenticateResponse> Authenticate(string emailOrUsername, string password,
        CancellationToken cancellationToken = default);
    Task<Guid> CreateAsync(CreateUserCommand user, CancellationToken cancellationToken = default);
    Task UpdateEmail(Guid userId, Email newEmail, CancellationToken cancellationToken = default);
    Task UpdateUsername(Guid userId, string username, CancellationToken cancellationToken = default);
    Task UpdatePassword(Guid userId, string oldPassword, string newPassword, CancellationToken cancellationToken);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    Task<GetUserByIdQueryResponse> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
}