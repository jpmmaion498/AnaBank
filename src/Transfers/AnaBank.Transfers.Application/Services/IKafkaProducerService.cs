namespace AnaBank.Transfers.Application.Services;

public interface IKafkaProducerService
{
    Task PublishTransferCompletedAsync(string transferId, string accountId, decimal amount, DateTime transferDate);
}