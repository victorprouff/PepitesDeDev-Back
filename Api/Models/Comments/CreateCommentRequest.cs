namespace Api.Models.Comments;

public record CreateCommentRequest(Guid NuggetId, string Content);