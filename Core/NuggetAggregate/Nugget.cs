namespace Core.NuggetAggregate;

public class Nugget
{
    public Nugget(string title, string description)
    {
        Id = Guid.NewGuid();
        Title = title;
        Description = description;
    }
    public Guid Id { get; }
    public string Title { get; private set; }
    public string Description { get; private set; }

    public void Update(string? title, string? description)
    {
        if (string.IsNullOrWhiteSpace(title) is false)
        {
            Title = title;
        }

        if (string.IsNullOrWhiteSpace(description) is false)
        {
            Description = description;
        }
    }
}