using Core.Interfaces;
using Core.NuggetAggregate;
using Dapper;
using Infrastructure.Entities;

namespace Infrastructure.Repositories;

public class NuggetRepository : BaseRepository, INuggetRepository
{
    public NuggetRepository(string connectionString) : base(connectionString)
    {
    }

    public async Task CreateAsync(Nugget nugget, CancellationToken cancellationToken = default)
    {
        var sql =
            @"INSERT INTO nuggets (id, title, description, created_at, updated_at) VALUES (@Id, @Title, @Description, @CreatedAt, @UpdatedAt);";

        await using var connection = GetConnection();
        await connection.ExecuteAsync(sql, (NuggetEntity)nugget, commandTimeout: 1);
    }

    public async Task UpdateAsync(Nugget nugget, CancellationToken cancellationToken = default)
    {
        await using var connection = GetConnection();
        var sql =
            @"UPDATE nuggets SET title = @Title, description = @Description, updated_at = @UpdatedAt WHERE id = @Id;";

        await connection.ExecuteAsync(
            sql,
            new
            {
                nugget.Id,
                nugget.Title,
                nugget.Description,
                nugget.UpdatedAt
            },
            commandTimeout: 1);
    }

    public async Task Delete(Guid id, CancellationToken cancellationToken = default)
    {
        var sql =
            @"DELETE FROM nuggets WHERE id = @Id;";

        await using var connection = GetConnection();
        await connection.ExecuteAsync(sql, new { Id = id }, commandTimeout: 1);
    }

    public async Task<IEnumerable<Nugget>> Get(CancellationToken cancellationToken = default)
    {
        var sql = @"SELECT id, title, description, created_at, updated_at FROM nuggets;";

        await using var connexion = GetConnection();
        var nuggets = await connexion.QueryAsync<NuggetEntity>(
            sql,
            commandTimeout: 1);

        return nuggets.Select(n => (Nugget)n);
    }

    public async Task<Nugget?> GetById(Guid id, CancellationToken cancellationToken = default)
    {
        var sql = @"SELECT id, title, description, created_at, updated_at FROM nuggets WHERE id = @Id";

        await using var connexion = GetConnection();
        return (Nugget?)await connexion.QueryFirstOrDefaultAsync<NuggetEntity?>(
            sql,
            new { Id = id },
            commandTimeout: 1);
    }
}