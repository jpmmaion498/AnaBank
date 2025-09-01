using AnaBank.Accounts.Domain.ValueObjects;

namespace AnaBank.Accounts.Domain.Entities;

public class CurrentAccount
{
    public string Id { get; private set; } = Guid.NewGuid().ToString();
    public string Name { get; private set; }
    public Cpf Cpf { get; private set; }
    public int Number { get; private set; }
    public string PasswordHash { get; private set; }
    public string Salt { get; private set; }
    public bool IsActive { get; private set; }
    public DateTime CreatedAt { get; private set; }

    private CurrentAccount() { } // EF Core

    public CurrentAccount(string name, Cpf cpf, int number, string passwordHash, string salt)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Nome não pode ser nulo ou vazio", nameof(name));
        
        if (string.IsNullOrWhiteSpace(passwordHash))
            throw new ArgumentException("Hash da senha não pode ser nulo ou vazio", nameof(passwordHash));
        
        if (string.IsNullOrWhiteSpace(salt))
            throw new ArgumentException("Salt não pode ser nulo ou vazio", nameof(salt));

        Name = name;
        Cpf = cpf;
        Number = number;
        PasswordHash = passwordHash;
        Salt = salt;
        IsActive = true;
        CreatedAt = DateTime.UtcNow;
    }

    public void Deactivate()
    {
        IsActive = false;
    }

    public void Activate()
    {
        IsActive = true;
    }
}