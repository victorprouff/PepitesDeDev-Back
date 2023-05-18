namespace Core.Services.Interfaces;

public interface IPasswordEncryptor
{
    string GenerateHash(string password, string salt);
    string GenerateSalt();
    bool VerifyPassword(string password, string salt, string passwordHash);
}