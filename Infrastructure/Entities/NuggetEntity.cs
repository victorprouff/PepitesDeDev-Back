using Core.NuggetAggregate;
using NodaTime;

namespace Infrastructure.Entities;

public class NuggetEntity
{

    public NuggetEntity()
    {
    }
    
    public NuggetEntity(Guid id, string title, string content, Guid userId, Instant createdAt, Instant? updatedAt)
    {
        Id = id;
        Title = title;
        Content = content;
        UserId = userId;
        CreatedAt = createdAt;
        UpdatedAt = updatedAt;
    }
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string Title { get; set; } = default!;
    public string Content { get; set; } = default!;
    public string? Creator { get; set; } = default!;
    public Instant CreatedAt { get; set; }
    public Instant? UpdatedAt { get; set; }
    

    public static explicit operator Core.NuggetAggregate.Projections.Nugget?(NuggetEntity? nugget) =>
        nugget is null
            ? null
            : new Core.NuggetAggregate.Projections.Nugget(
                nugget.Id,
                nugget.Title,
                nugget.Content,
                nugget.Creator,
                nugget.UserId,
                nugget.CreatedAt,
                nugget.UpdatedAt);
    
    public static explicit operator Nugget?(NuggetEntity? nugget) =>
        nugget is null
            ? null
            : new Nugget(
                nugget.Id,
                nugget.Title,
                nugget.Content,
                nugget.UserId,
                nugget.CreatedAt,
                nugget.UpdatedAt);

    public static explicit operator NuggetEntity(Nugget nugget) =>
        new(nugget.Id, nugget.Title, nugget.Content, nugget.UserId, nugget.CreatedAt, nugget.UpdatedAt);
}