using Core.CommentAggregate;
using Core.Interfaces;
using Dapper;
using Infrastructure.Entities;

namespace Infrastructure.Repositories;

public class CommentRepository : BaseRepository, ICommentRepository
{
    public CommentRepository(string connectionString) : base(connectionString)
    {
    }

    public async Task CreateAsync(Comment comment, CancellationToken cancellationToken)
    {
        const string sql =
            "INSERT INTO comments (id, user_id, nugget_id, content, created_at, updated_at) VALUES (@Id, @UserId, @NuggetId, @Content, @CreatedAt, @UpdatedAt);";

        await using var connection = GetConnection();
        await connection.ExecuteAsync(sql, (CommentEntity)comment, commandTimeout: 1);
    }
}