using Core.CommentAggregate.Models;

namespace Core.CommentAggregate;

public interface ICommentDomain
{
    Task<Guid> CreateAsync(CreateCommentCommand command, CancellationToken cancellationToken);
}