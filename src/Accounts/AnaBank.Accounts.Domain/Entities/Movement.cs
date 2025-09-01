namespace AnaBank.Accounts.Domain.Entities;

public class Movement
{
    public string Id { get; private set; } = Guid.NewGuid().ToString();
    public string AccountId { get; private set; }
    public string Type { get; private set; } // "C" ou "D"
    public decimal Value { get; private set; }
    public string MovementDate { get; private set; } // Formato DD/MM/YYYY
    public DateTime CreatedAt { get; private set; }

    private Movement() { } // EF Core

    public Movement(string accountId, string type, decimal value)
    {
        if (string.IsNullOrWhiteSpace(accountId))
            throw new ArgumentException("ID da conta não pode ser nulo ou vazio", nameof(accountId));

        if (string.IsNullOrWhiteSpace(type) || (type != "C" && type != "D"))
            throw new ArgumentException("Tipo deve ser 'C' ou 'D'", nameof(type));

        if (value <= 0)
            throw new ArgumentException("Valor deve ser maior que zero", nameof(value));

        AccountId = accountId;
        Type = type;
        Value = value;
        MovementDate = DateTime.UtcNow.ToString("dd/MM/yyyy");
        CreatedAt = DateTime.UtcNow;
    }

    public bool IsCredit => Type == "C";
    public bool IsDebit => Type == "D";
}