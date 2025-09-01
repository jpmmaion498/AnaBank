namespace AnaBank.Accounts.Domain.ValueObjects;

public record AccountNumber
{
    public string Value { get; }

    public AccountNumber(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Número da conta não pode ser nulo ou vazio", nameof(value));

        Value = value;
    }

    public override string ToString() => Value;

    public static implicit operator string(AccountNumber accountNumber) => accountNumber.Value;
    public static implicit operator AccountNumber(string value) => new(value);
}