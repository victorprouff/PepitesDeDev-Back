using Core.UserAggregate;
using NodaTime;

namespace Infrastructure.Entities;

public class UserEntity
{
    public UserEntity()
    {
    }

    public UserEntity(Guid id, string email, string password, string salt, Instant createdAt, Instant? updatedAt)
    {
        Id = id;
        Email = email;
        Password = password;
        Salt = salt;
        CreatedAt = createdAt;
        UpdatedAt = updatedAt;
    }
    
    public Guid Id { get; set; }
    public string Email { get; set; } = default!;
    public string? Password { get; set; } = default!;
    public string? Salt { get; set; } = default!;
    public Instant CreatedAt { get; set; }
    public Instant? UpdatedAt { get; set; }

    public static explicit operator User?(UserEntity? user) =>
        user is null
            ? null
            : new User(user.Id, new Email(user.Email), user.Password, user.Salt, user.CreatedAt, user.UpdatedAt);

    public static explicit operator UserEntity(User user) =>
        new(user.Id, user.Email.Value, user.Password, user.Salt,  user.CreatedAt, user.UpdatedAt);
}