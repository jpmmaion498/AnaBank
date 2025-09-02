using AnaBank.Fees.Worker.Services;

namespace AnaBank.Fees.Worker;

public class FeeProcessingService : BackgroundService
{
    private readonly ILogger<FeeProcessingService> _logger;
    private readonly IServiceProvider _serviceProvider;

    public FeeProcessingService(ILogger<FeeProcessingService> logger, IServiceProvider serviceProvider)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Fee Processing Service started - Kafka Consumer Mode");

        try
        {
            using var scope = _serviceProvider.CreateScope();
            var kafkaConsumer = scope.ServiceProvider.GetRequiredService<IKafkaConsumerService>();
            
            await kafkaConsumer.StartConsumingAsync(stoppingToken);
        }
        catch (OperationCanceledException)
        {
            _logger.LogInformation("Fee Processing Service stopped - cancellation requested");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Fatal error in Fee Processing Service");
            throw;
        }

        _logger.LogInformation("Fee Processing Service stopped");
    }
}