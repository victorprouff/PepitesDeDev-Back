using NodaTime;

namespace Core.UserAggregate;

public class User
{
    public User(Guid id, Email email, string username, string password, string salt, bool isAdmin, Instant createdAt, Instant? updatedAt)
    {
        Id = id;
        Email = email;
        Username = username;
        Password = password;
        Salt = salt;
        IsAdmin = isAdmin;
        CreatedAt = createdAt;
        UpdatedAt = updatedAt;
    }
    
    public Guid Id { get; }
    public Email Email { get; }
    public string Username { get; }
    public string Password { get; }
    public string Salt { get; }
    public bool IsAdmin { get; set; }
    public Instant CreatedAt { get; private set; }
    public Instant? UpdatedAt { get; private set; }

    public static User Create(Email email, string username, string password, string salt, bool isAdmin, Instant createdAt) =>
        new(Guid.NewGuid(), email, username, password, salt, isAdmin, createdAt, null);
}