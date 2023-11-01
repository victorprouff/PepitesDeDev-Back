using Core.CommentAggregate.Models;
using Core.Interfaces;
using NodaTime;

namespace Core.CommentAggregate;

public class CommentDomain : ICommentDomain
{
    private readonly IClock _clock;
    private readonly ICommentRepository _commentRepository;

    public CommentDomain(IClock clock, ICommentRepository commentRepository)
    {
        _clock = clock;
        _commentRepository = commentRepository;
    }

    public async Task<Guid> CreateAsync(CreateCommentCommand command, CancellationToken cancellationToken)
    {
        var newComment = Comment.Create(command.NuggetId, command.UserId, command.Content, _clock.GetCurrentInstant());

        await _commentRepository.CreateAsync(newComment, cancellationToken);

        return newComment.Id;
    }
}