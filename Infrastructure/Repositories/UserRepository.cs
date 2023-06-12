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

    public async Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        var sql = @"
                SELECT id, username, email, password, salt, created_at, updated_at FROM users WHERE email = @Email;";

        await using var connexion = GetConnection();
        return (User?)await connexion.QueryFirstOrDefaultAsync<UserEntity?>(
            sql, new { email }, commandTimeout: 1);
    }
    
    public async Task<User?> GetByEmailOrUsernameAsync(string emailOrUsername, CancellationToken cancellationToken = default)
    {
        var sql = @"
                SELECT id, username, email, password, salt, created_at, updated_at FROM users WHERE email = @EmailOrUsername OR username = @EmailOrUsername;";

        await using var connexion = GetConnection();
        return (User?)await connexion.QueryFirstOrDefaultAsync<UserEntity?>(
            sql, new { emailOrUsername }, commandTimeout: 1);
    }

    public async Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var sql = @"SELECT id, email, username, created_at, updated_at FROM users WHERE id = @Id";

        await using var connexion = GetConnection();
        return (User?)await connexion.QueryFirstOrDefaultAsync<UserEntity?>(
            sql,
            new { Id = id },
            commandTimeout: 1);
    }
    
    public async Task CreateAsync(User user, CancellationToken cancellationToken = default)
    {
        var sql =
            @"INSERT INTO users (id, email, username, password, salt, created_at, updated_at) VALUES (@Id, @Email, @Username, @Password, @Salt, @CreatedAt, @UpdatedAt);";

        await using var connection = GetConnection();
        await connection.ExecuteAsync(sql, (UserEntity)user, commandTimeout: 1);
    }

    public async Task UpdateEmail(Guid userId, string newEmailValue, CancellationToken cancellationToken)
    {
        var sql =
            @"UPDATE users SET email = @Email WHERE id = @Id;";

        await using var connection = GetConnection();
        await connection.ExecuteAsync(sql, new { id = userId, email = newEmailValue }, commandTimeout: 1);
    }

    public async Task UpdateUsername(Guid userId, string username, CancellationToken cancellationToken)
    {
        var sql =
            @"UPDATE users SET username = @Username WHERE id = @Id;";

        await using var connection = GetConnection();
        await connection.ExecuteAsync(sql, new { id = userId, username = username }, commandTimeout: 1);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var sql =
            @"DELETE FROM users WHERE id = @Id;";

        await using var connection = GetConnection();
        await connection.ExecuteAsync(sql, new { Id = id }, commandTimeout: 1);
    }
}