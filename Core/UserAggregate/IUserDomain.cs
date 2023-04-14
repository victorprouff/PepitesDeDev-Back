using Core.UserAggregate.Models;

namespace Core.UserAggregate;

public interface IUserDomain
{
    Task<Guid> CreateAsync(CreateUserCommand user, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    Task<GetUserByIdQueryResponse?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
}