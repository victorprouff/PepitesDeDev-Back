namespace Core.CommentAggregate.Models;

public record CreateCommentCommand(Guid NuggetId, Guid UserId, string Content);