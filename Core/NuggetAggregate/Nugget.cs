using NodaTime;

namespace Core.NuggetAggregate;

public class Nugget
{
    public Nugget(Guid id, string title, string content, string? urlImage, Guid userId, Instant createdAt, Instant? updatedAt)
    {
        Id = id;
        Title = title;
        Content = content;
        UrlImage = urlImage;
        CreatedAt = createdAt;
        UpdatedAt = updatedAt;
        UserId = userId;
    }

    public Guid Id { get; }
    public Guid UserId { get; }

    public string Title { get; private set; }
    public string Content { get; private set; }
    public string? UrlImage { get; private set; }
    public Instant CreatedAt { get; private set; }
    public Instant? UpdatedAt { get; private set; }
    
    public static Nugget Create(string title, string content, Guid userId, Instant createdAt) =>
        new(Guid.NewGuid(), title, content, null, userId, createdAt, null);
    
    public void Update(string? title, string? content, Instant updatedAt)
    {
        if (string.IsNullOrWhiteSpace(title) is false)
        {
            Title = title;
            UpdatedAt = updatedAt;
        }

        if (string.IsNullOrWhiteSpace(content) is false)
        {
            Content = content;
            UpdatedAt = updatedAt;
        }
    }

    public void UpdateUrlImage(string? urlImage, Instant updatedAt)
    {
        UrlImage = urlImage;
        UpdatedAt = updatedAt;
    }
}