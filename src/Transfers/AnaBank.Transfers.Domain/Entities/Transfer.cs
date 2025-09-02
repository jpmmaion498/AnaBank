namespace AnaBank.Transfers.Domain.Entities;

public class Transfer
{
    public string Id { get; private set; } = Guid.NewGuid().ToString();
    public string OriginAccountId { get; private set; }
    public string DestinationAccountId { get; private set; }
    public decimal Value { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public string Status { get; private set; } = "COMPLETED";

    private Transfer() { }

    public Transfer(string originAccountId, string destinationAccountId, decimal value)
    {
        if (string.IsNullOrWhiteSpace(originAccountId))
            throw new ArgumentException("ID da conta de origem não pode ser nulo ou vazio", nameof(originAccountId));

        if (string.IsNullOrWhiteSpace(destinationAccountId))
            throw new ArgumentException("ID da conta de destino não pode ser nulo ou vazio", nameof(destinationAccountId));

        if (originAccountId == destinationAccountId)
            throw new ArgumentException("Conta de origem e destino não podem ser iguais");

        if (value <= 0)
            throw new ArgumentException("Valor deve ser maior que zero", nameof(value));

        OriginAccountId = originAccountId;
        DestinationAccountId = destinationAccountId;
        Value = value;
        CreatedAt = DateTime.UtcNow;
        Status = "COMPLETED";
    }

    public void MarkAsFailed()
    {
        Status = "FAILED";
    }

    public void MarkAsReversed()
    {
        Status = "REVERSED";
    }
}