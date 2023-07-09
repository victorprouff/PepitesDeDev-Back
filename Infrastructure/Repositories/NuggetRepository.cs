using Core.Interfaces;
using Core.NuggetAggregate.Projections;
using Dapper;
using Infrastructure.Entities;
using Nugget = Core.NuggetAggregate.Nugget;

namespace Infrastructure.Repositories;

public class NuggetRepository : BaseRepository, INuggetRepository
{
    public NuggetRepository(string connectionString) : base(connectionString)
    {
    }

    public async Task CreateAsync(Nugget nugget, CancellationToken cancellationToken)
    {
        const string sql =
            @"INSERT INTO nuggets (id, title, content, user_id, created_at, updated_at) VALUES (@Id, @Title, @Content, @UserId, @CreatedAt, @UpdatedAt);";

        await using var connection = GetConnection();
        await connection.ExecuteAsync(sql, (NuggetEntity)nugget, commandTimeout: 1);
    }

    public async Task UpdateAsync(Nugget nugget, CancellationToken cancellationToken)
    {
        await using var connection = GetConnection();
        const string sql = @"UPDATE nuggets SET title = @Title, content = @Content, updated_at = @UpdatedAt WHERE id = @Id;";

        await connection.ExecuteAsync(
            sql,
            new
            {
                nugget.Id,
                nugget.Title,
                nugget.Content,
                nugget.UpdatedAt
            },
            commandTimeout: 1);
    }

    public async Task UpdateUrlImageAsync(Nugget nugget, CancellationToken cancellationToken)
    {
        await using var connection = GetConnection();
        const string sql = @"UPDATE nuggets SET url_image = @UrlImage, updated_at = @UpdatedAt WHERE id = @Id;";

        await connection.ExecuteAsync(
            sql,
            new
            {
                nugget.Id,
                nugget.UrlImage,
                nugget.UpdatedAt
            },
            commandTimeout: 1);
    }

    public async Task Delete(Guid id, CancellationToken cancellationToken)
    {
        const string sql = @"DELETE FROM nuggets WHERE id = @Id;";

        await using var connection = GetConnection();
        await connection.ExecuteAsync(sql, new { Id = id }, commandTimeout: 1);
    }

    public async Task<GetAllNuggetsProjection> GetAll(int limit, int offset, CancellationToken cancellationToken)
    {
        const string sql = @"
            SELECT count(*) FROM nuggets;
            SELECT n.id, n.title, n.content, n.user_id, u.username AS creator, n.created_at, n.updated_at
            FROM nuggets n
                LEFT OUTER JOIN users u on n.user_id = u.id
            ORDER BY created_at
            DESC LIMIT @Limit OFFSET @Offset;";

        await using var connexion = GetConnection();
        using var multi = await connexion.QueryMultipleAsync(
            sql,
            new { limit, offset },
            commandTimeout: 1);
        
        var nbOfNuggets = multi.Read<int>().Single();
        var nuggets = await multi.ReadAsync<NuggetEntity>();
            
        return new GetAllNuggetsProjection(
            nbOfNuggets,
            nuggets.Select(n => (Core.NuggetAggregate.Projections.Nugget)n));
    }

    public async Task<GetAllNuggetsProjection> GetAllByUserIdProjection(Guid userId, int limit, int offset, CancellationToken cancellationToken)
    {
        const string sql = @"
            SELECT count(*) FROM nuggets WHERE user_id = @UserId;
            SELECT n.id, n.title, n.content, n.user_id, u.username AS creator, n.created_at, n.updated_at
            FROM nuggets n
                LEFT OUTER JOIN users u on n.user_id = u.id
            WHERE n.user_id = @UserId
            ORDER BY n.created_at DESC
            LIMIT @Limit OFFSET @Offset;";

        await using var connexion = GetConnection();
        using var multi = await connexion.QueryMultipleAsync(
            sql,
            new { UserId = userId, Limit = limit, Offset = offset },
            commandTimeout: 1);
        
        var nbOfNuggets = multi.Read<int>().Single();
        var nuggets = await multi.ReadAsync<NuggetEntity>();
            
        return new GetAllNuggetsProjection(
            nbOfNuggets,
            nuggets.Select(n => (Core.NuggetAggregate.Projections.Nugget)n));
    }
    
    public async Task<GetNuggetProjection?> GetByIdProjection(Guid id, CancellationToken cancellationToken)
    {
        const string sql = @"
            SELECT n.id, n.title, n.content, n.user_id, u.username AS creator, n.created_at, n.updated_at
            FROM nuggets n
                LEFT OUTER JOIN users u on n.user_id = u.id
            WHERE n.id = @Id";

        await using var connexion = GetConnection();
        return await connexion.QueryFirstOrDefaultAsync<GetNuggetProjection?>(
            sql,
            new { Id = id },
            commandTimeout: 1);
    }
    
    public async Task<Nugget?> GetById(Guid id, CancellationToken cancellationToken)
    {
        const string sql = @"
            SELECT n.id, n.title, n.content, n.user_id, u.username AS creator, n.created_at, n.updated_at
            FROM nuggets n
                LEFT OUTER JOIN users u on n.user_id = u.id
            WHERE n.id = @Id";
        
        await using var connexion = GetConnection();
        return (Nugget?)await connexion.QueryFirstOrDefaultAsync<NuggetEntity?>(
            sql,
            new { Id = id },
            commandTimeout: 1);
    }
}