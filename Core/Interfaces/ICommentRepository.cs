using Comment = Core.CommentAggregate.Comment;

namespace Core.Interfaces;

public interface ICommentRepository
{
    Task CreateAsync(Comment comment, CancellationToken cancellationToken);
}