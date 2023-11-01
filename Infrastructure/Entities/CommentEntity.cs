using Core.CommentAggregate;
using NodaTime;

namespace Infrastructure.Entities;

public class CommentEntity
{
    public CommentEntity()
    {
    }

    public CommentEntity(Guid id, Guid nuggetId, Guid userId, string content, Instant createdAt, Instant? updatedAt)
    {
        Id = id;
        NuggetId = nuggetId;
        UserId = userId;
        Content = content;
        CreatedAt = createdAt;
        UpdatedAt = updatedAt;
    }

    public Guid Id { get; }
    public Guid NuggetId { get; }
    public Guid UserId { get; }
    public string Content { get; } = default!;
    public Instant CreatedAt { get; }
    public Instant? UpdatedAt { get; }

    public static explicit operator Comment?(CommentEntity? comment) =>
        comment is null
            ? null
            : new Comment(
                comment.Id,
                comment.NuggetId,
                comment.UserId,
                comment.Content,
                comment.CreatedAt,
                comment.UpdatedAt);

    public static explicit operator CommentEntity(Comment comment) =>
        new(
                comment.Id,
                comment.NuggetId,
                comment.UserId,
                comment.Content,
                comment.CreatedAt,
                comment.UpdatedAt);
}