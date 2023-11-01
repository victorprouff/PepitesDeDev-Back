using NodaTime;

namespace Core.CommentAggregate;

public class Comment
{
    public Comment(Guid id, Guid nuggetId, Guid userId, string content, Instant createdAt, Instant? updatedAt)
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
    public string Content { get; }
    public Instant CreatedAt { get; }
    public Instant? UpdatedAt { get; }

    public static Comment Create(Guid nuggetId, Guid userId, string content, Instant createdAt)
        => new(Guid.NewGuid(), nuggetId, userId, content, createdAt, null);
}