using NodaTime;

namespace Core.UserAggregate;

public class User
{
    public User(Guid id, string email, string password, string salt, Instant createdAt, Instant? updatedAt)
    {
        Id = id;
        Email = email;
        Password = password;
        Salt = salt;
        CreatedAt = createdAt;
        UpdatedAt = updatedAt;
    }
    
    public Guid Id { get; }
    public string Email { get; }
    public string? Password { get; }
    public string? Salt { get; }
    public Instant CreatedAt { get; private set; }
    public Instant? UpdatedAt { get; private set; }

    public static User Create(string email, string password, string salt, Instant createdAt) =>
        new(Guid.NewGuid(), email, password, salt, createdAt, null);
}