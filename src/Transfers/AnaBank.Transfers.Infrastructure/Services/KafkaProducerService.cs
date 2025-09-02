using Confluent.Kafka;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace AnaBank.Transfers.Infrastructure.Services;

public interface IKafkaProducerService
{
    Task PublishTransferCompletedAsync(string transferId, string accountId, decimal amount, DateTime transferDate);
}

public class KafkaProducerService : IKafkaProducerService, IDisposable
{
    private readonly IProducer<Null, string> _producer;
    private readonly ILogger<KafkaProducerService> _logger;

    public KafkaProducerService(IConfiguration configuration, ILogger<KafkaProducerService> logger)
    {
        _logger = logger;
        
        var kafkaConnectionString = configuration.GetConnectionString("Kafka") ?? "localhost:9092";
        
        var config = new ProducerConfig
        {
            BootstrapServers = kafkaConnectionString,
            Acks = Acks.All,
            MessageTimeoutMs = 10000,
            RequestTimeoutMs = 5000
        };

        _producer = new ProducerBuilder<Null, string>(config).Build();
        
        _logger.LogInformation($"Kafka producer initialized with servers: {kafkaConnectionString}");
    }

    public async Task PublishTransferCompletedAsync(string transferId, string accountId, decimal amount, DateTime transferDate)
    {
        try
        {
            var message = new
            {
                TransferId = transferId,
                AccountId = accountId,
                TransferAmount = amount,
                TransferDate = transferDate
            };

            var messageJson = JsonSerializer.Serialize(message);
            
            var result = await _producer.ProduceAsync("transfer-completed", new Message<Null, string>
            {
                Value = messageJson
            });

            _logger.LogInformation($"Transfer completed message published successfully. Topic: {result.Topic}, Partition: {result.Partition}, Offset: {result.Offset}");
        }
        catch (ProduceException<Null, string> ex)
        {
            _logger.LogError(ex, $"Failed to publish transfer completed message for transfer {transferId}");
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Unexpected error publishing transfer completed message for transfer {transferId}");
            throw;
        }
    }

    public void Dispose()
    {
        _producer?.Dispose();
    }
}