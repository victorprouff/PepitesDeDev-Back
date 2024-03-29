using NodaTime;

namespace Core.NuggetAggregate.Projections;

public class GetNuggetProjection
{
    public GetNuggetProjection()
    {
    }
    
    public GetNuggetProjection(
        Guid id,
        Guid userId,
        string title,
        string content,
        bool isEnabled,
        string? urlImage,
        string creator,
        Instant createdAt,
        Instant? updatedAt)
    {
        Id = id;
        UserId = userId;
        Title = title;
        Content = content;
        IsEnabled = isEnabled;
        UrlImage = urlImage;
        Creator = creator;
        CreatedAt = createdAt;
        UpdatedAt = updatedAt;
    }
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string Title { get; set; } = default!;
    public string Content { get; set; } = default!;
    public bool IsEnabled { get; set; }
    public string Creator { get; set; } = default!;
    public string? UrlImage { get; set; } = default!;
    public Instant CreatedAt { get; set; }
    public Instant? UpdatedAt { get; set; }
}