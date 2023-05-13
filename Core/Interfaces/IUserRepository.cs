using Core.UserAggregate;

namespace Core.Interfaces;

public interface IUserRepository
{
    Task<Guid?> Authenticate(string email, string password, CancellationToken cancellationToken = default);
    Task CreateAsync(User user, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
}