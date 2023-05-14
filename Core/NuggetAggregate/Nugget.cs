using NodaTime;

namespace Core.NuggetAggregate;

public class Nugget
{
    public Nugget(Guid id, string title, string content, Instant createdAt, Instant? updatedAt)
    {
        Id = id;
        Title = title;
        Content = content;
        CreatedAt = createdAt;
        UpdatedAt = updatedAt;
    }

    public Guid Id { get; }
    public string Title { get; private set; }
    public string Content { get; private set; }
    public Instant CreatedAt { get; private set; }
    public Instant? UpdatedAt { get; private set; }
    
    public static Nugget Create(string title, string content, Instant createdAt) =>
        new(Guid.NewGuid(), title, content, createdAt, null);
    
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
}