using AnaBank.Accounts.Application.Commands.RegisterAccount;
using System.Security.Cryptography;
using System.Text;

namespace AnaBank.Accounts.Infrastructure.Services;

public class PasswordHasher : IPasswordHasher
{
    public string HashPassword(string password, string salt)
    {
        using var sha256 = SHA256.Create();
        var saltedPassword = password + salt;
        var saltedPasswordBytes = Encoding.UTF8.GetBytes(saltedPassword);
        var hashBytes = sha256.ComputeHash(saltedPasswordBytes);
        return Convert.ToBase64String(hashBytes);
    }

    public bool VerifyPassword(string password, string hash, string salt)
    {
        var computedHash = HashPassword(password, salt);
        return computedHash == hash;
    }
}