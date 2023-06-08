using NodaTime;

namespace Core.UserAggregate;

public class User
{
    public User(Guid id, Email email, string password, string salt, Instant createdAt, Instant? updatedAt)
    {
        Id = id;
        Email = email;
        Password = password;
        Salt = salt;
        CreatedAt = createdAt;
        UpdatedAt = updatedAt;
    }
    
    public Guid Id { get; }
    public Email Email { get; }
    public string? Password { get; }
    public string? Salt { get; }
    public Instant CreatedAt { get; private set; }
    public Instant? UpdatedAt { get; private set; }

    public static User Create(Email email, string password, string salt, Instant createdAt) =>
        new(Guid.NewGuid(), email, password, salt, createdAt, null);
}