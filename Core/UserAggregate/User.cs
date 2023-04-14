using NodaTime;

namespace Core.UserAggregate;

public class User
{
    public User(Guid id, string email, string password, Instant createdAt, Instant? updatedAt)
    {
        Id = id;
        Email = email;
        Password = password;
        CreatedAt = createdAt;
        UpdatedAt = updatedAt;
    }
    
    public Guid Id { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public Instant CreatedAt { get; private set; }
    public Instant? UpdatedAt { get; private set; }

    public static User Create(string email, string password, Instant createdAt) =>
        new(Guid.NewGuid(), email, password, createdAt, null);
}