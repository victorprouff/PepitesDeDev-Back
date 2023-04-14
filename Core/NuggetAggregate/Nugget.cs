using NodaTime;

namespace Core.NuggetAggregate;

public class Nugget
{
    public Nugget(Guid id, string title, string description, Instant createdAt, Instant? updatedAt)
    {
        Id = id;
        Title = title;
        Description = description;
        CreatedAt = createdAt;
        UpdatedAt = updatedAt;
    }

    public Guid Id { get; }
    public string Title { get; private set; }
    public string Description { get; private set; }
    public Instant CreatedAt { get; private set; }
    public Instant? UpdatedAt { get; private set; }
    
    public static Nugget Create(string title, string description, Instant createdAt) =>
        new(Guid.NewGuid(), title, description, createdAt, null);
    
    public void Update(string? title, string? description, Instant updatedAt)
    {
        if (string.IsNullOrWhiteSpace(title) is false)
        {
            Title = title;
            UpdatedAt = updatedAt;
        }

        if (string.IsNullOrWhiteSpace(description) is false)
        {
            Description = description;
            UpdatedAt = updatedAt;
        }
    }
}