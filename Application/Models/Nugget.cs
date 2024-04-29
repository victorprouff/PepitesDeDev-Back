using NodaTime;

namespace Application.Models;

public class Nugget
{
    public Nugget(
        Guid id,
        string title,
        string content,
        bool isEnabled,
        string? urlImage,
        string creator,
        Guid userId,
        Instant createdAt,
        Instant? updatedAt)
    {
        Id = id;
        Title = title;
        Content = content;
        IsEnabled = isEnabled;
        UrlImage = urlImage;
        Creator = creator;
        CreatedAt = createdAt;
        UpdatedAt = updatedAt;
        UserId = userId;
    }

    public Guid Id { get; }
    public Guid UserId { get; }

    public string Title { get; private set; }
    public string Content { get; private set; }
    public bool IsEnabled { get; private set; }
    public string? UrlImage { get; private set; }
    public string Creator { get; private set; }
    public Instant CreatedAt { get; private set; }
    public Instant? UpdatedAt { get; private set; }
}