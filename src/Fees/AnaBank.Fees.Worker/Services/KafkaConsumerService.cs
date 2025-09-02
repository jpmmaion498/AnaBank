using Confluent.Kafka;
using System.Text.Json;
using AnaBank.Fees.Worker.Models;
using AnaBank.Fees.Worker.Services;

namespace AnaBank.Fees.Worker.Services;

public interface IKafkaConsumerService
{
    Task StartConsumingAsync(CancellationToken cancellationToken);
}

public class KafkaConsumerService : IKafkaConsumerService
{
    private readonly ILogger<KafkaConsumerService> _logger;
    private readonly IServiceProvider _serviceProvider;
    private readonly IConfiguration _configuration;

    public KafkaConsumerService(
        ILogger<KafkaConsumerService> logger,
        IServiceProvider serviceProvider,
        IConfiguration configuration)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
        _configuration = configuration;
    }

    public async Task StartConsumingAsync(CancellationToken cancellationToken)
    {
        var kafkaConnectionString = _configuration.GetConnectionString("Kafka") ?? "localhost:9092";
        
        var config = new ConsumerConfig
        {
            BootstrapServers = kafkaConnectionString,
            GroupId = "anabank-fees-worker",
            AutoOffsetReset = AutoOffsetReset.Earliest,
            EnableAutoCommit = false
        };

        using var consumer = new ConsumerBuilder<Ignore, string>(config).Build();
        
        consumer.Subscribe("transfer-completed");
        _logger.LogInformation("Kafka consumer started. Subscribed to 'transfer-completed' topic");

        try
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    var consumeResult = consumer.Consume(TimeSpan.FromSeconds(10));
                    
                    if (consumeResult?.Message?.Value != null)
                    {
                        _logger.LogInformation($"Received message: {consumeResult.Message.Value}");
                        
                        await ProcessTransferMessage(consumeResult.Message.Value);
                        
                        consumer.Commit(consumeResult);
                        _logger.LogDebug("Message committed successfully");
                    }
                }
                catch (ConsumeException ex)
                {
                    _logger.LogError(ex, "Error consuming message from Kafka");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error processing transfer message");
                }
            }
        }
        finally
        {
            consumer.Close();
            _logger.LogInformation("Kafka consumer closed");
        }
    }

    private async Task ProcessTransferMessage(string messageValue)
    {
        try
        {
            var transferMessage = JsonSerializer.Deserialize<TransferCompletedMessage>(messageValue);
            
            if (transferMessage == null)
            {
                _logger.LogWarning("Failed to deserialize transfer message");
                return;
            }

            _logger.LogInformation($"Processing fee for transfer {transferMessage.TransferId}, account {transferMessage.AccountId}");

            using var scope = _serviceProvider.CreateScope();
            var feeService = scope.ServiceProvider.GetRequiredService<IFeeService>();

            await feeService.ProcessTransferFeeAsync(
                transferMessage.AccountId,
                transferMessage.TransferAmount,
                transferMessage.TransferDate);

            _logger.LogInformation($"Fee processed successfully for account {transferMessage.AccountId}");
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "Error deserializing transfer message");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing transfer fee");
        }
    }
}