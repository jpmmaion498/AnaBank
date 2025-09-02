using Confluent.Kafka;
using System.Text.Json;
using AnaBank.Accounts.Application.Commands.MakeMovement;
using MediatR;

namespace AnaBank.Accounts.API.Services;

public interface IFeeConsumerService
{
    Task StartConsumingAsync(CancellationToken cancellationToken);
}

public class FeeConsumerService : IFeeConsumerService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IConfiguration _configuration;
    private readonly ILogger<FeeConsumerService> _logger;

    public FeeConsumerService(
        IServiceProvider serviceProvider,
        IConfiguration configuration,
        ILogger<FeeConsumerService> logger)
    {
        _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task StartConsumingAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Starting Fee Consumer Service...");
        
        try
        {
            var config = new ConsumerConfig
            {
                GroupId = "accounts-fee-consumer",
                BootstrapServers = _configuration.GetConnectionString("Kafka") ?? "localhost:9092",
                AutoOffsetReset = AutoOffsetReset.Latest,
                EnableAutoCommit = true,
                SessionTimeoutMs = 6000,
                EnableAutoOffsetStore = false
            };

            using var consumer = new ConsumerBuilder<string, string>(config).Build();
            
            consumer.Subscribe("fee-completed");
            _logger.LogInformation("Fee consumer started. Subscribed to 'fee-completed' topic");

            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    var consumeResult = consumer.Consume(TimeSpan.FromSeconds(1));
                    
                    if (consumeResult != null)
                    {
                        _logger.LogInformation("Received fee message: {Message}", consumeResult.Message.Value);

                        var feeEvent = JsonSerializer.Deserialize<FeeCompletedEvent>(consumeResult.Message.Value);
                        if (feeEvent != null)
                        {
                            _logger.LogInformation("Processing fee debit for account {AccountId}, fee {FeeId}", 
                                feeEvent.AccountId, feeEvent.FeeId);

                            await ProcessFeeDebit(feeEvent);
                            
                            consumer.StoreOffset(consumeResult);
                            _logger.LogDebug("Fee message committed successfully");
                        }
                    }
                }
                catch (ConsumeException ex)
                {
                    _logger.LogWarning(ex, "Error consuming fee message from Kafka - retrying...");
                    await Task.Delay(5000, cancellationToken); // Aguardar mais tempo em caso de erro
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Error processing fee message - retrying...");
                    await Task.Delay(2000, cancellationToken);
                }
            }
        }
        catch (OperationCanceledException)
        {
            _logger.LogInformation("Fee Consumer Service was cancelled");
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Critical error in Fee Consumer Service. Service will continue without Kafka fee processing.");
            // Não quebrar a aplicação se Kafka não estiver disponível
        }
        finally
        {
            _logger.LogInformation("Fee consumer stopped");
        }
    }

    private async Task ProcessFeeDebit(FeeCompletedEvent feeEvent)
    {
        try
        {
            using var scope = _serviceProvider.CreateScope();
            var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

            var command = new MakeMovementCommand(
                AccountNumber: null,
                Type: "D", // Débito
                Value: feeEvent.FeeAmount,
                AccountId: feeEvent.AccountId
            );

            await mediator.Send(command);
            
            _logger.LogInformation("Fee successfully debited from account {AccountId}", feeEvent.AccountId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error debiting fee from account {AccountId}", feeEvent.AccountId);
            throw;
        }
    }
}

public class FeeCompletedEvent
{
    public string FeeId { get; set; } = string.Empty;
    public string AccountId { get; set; } = string.Empty;
    public decimal FeeAmount { get; set; }
    public DateTime ProcessedDate { get; set; }
}