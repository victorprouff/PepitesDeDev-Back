using Core.Interfaces;
using Core.UserAggregate;
using Dapper;
using Infrastructure.Entities;

namespace Infrastructure.Repositories;

public class UserRepository : BaseRepository, IUserRepository
{
    public UserRepository(string connectionString) : base(connectionString)
    {
    }

    public async Task CreateAsync(User user, CancellationToken cancellationToken = default)
    {
        var sql =
            @"INSERT INTO users (id, email, password, created_at, updated_at) VALUES (@Id, @Email, @Password, @CreatedAt, @UpdatedAt);";

        await using var connection = GetConnection();
        await connection.ExecuteAsync(sql, (UserEntity)user, commandTimeout: 1);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var sql =
            @"DELETE FROM users WHERE id = @Id;";

        await using var connection = GetConnection();
        await connection.ExecuteAsync(sql, new { Id = id }, commandTimeout: 1);
    }

    public async Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var sql = @"SELECT id, email, password, created_at, updated_at FROM users WHERE id = @Id";

        await using var connexion = GetConnection();
        return (User?)await connexion.QueryFirstOrDefaultAsync<UserEntity?>(
            sql,
            new { Id = id },
            commandTimeout: 1);
    }
}