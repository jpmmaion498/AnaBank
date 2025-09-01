namespace AnaBank.Fees.Worker.Models;

public class TransferCompletedMessage
{
    public string TransferId { get; set; } = string.Empty;
    public string AccountId { get; set; } = string.Empty;
    public decimal TransferAmount { get; set; }
    public DateTime TransferDate { get; set; }
}

public class FeeChargedMessage
{
    public string AccountId { get; set; } = string.Empty;
    public decimal FeeAmount { get; set; }
    public DateTime ChargedAt { get; set; }
}