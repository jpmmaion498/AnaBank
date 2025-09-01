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
        _logger.LogInformation("Fee Processing Service started");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                // Em uma implementação real, aqui consumiríamos mensagens do Kafka
                // Por enquanto, simularemos o processamento a cada 30 segundos
                await ProcessPendingFees();

                await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);
            }
            catch (OperationCanceledException)
            {
                // Expected when cancellation is requested
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while processing fees");
                await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
            }
        }

        _logger.LogInformation("Fee Processing Service stopped");
    }

    private async Task ProcessPendingFees()
    {
        using var scope = _serviceProvider.CreateScope();
        var feeService = scope.ServiceProvider.GetRequiredService<IFeeService>();

        // Simular processamento de tarifa
        // Em uma implementação real, isso seria baseado em mensagens do Kafka
        _logger.LogInformation("Checking for pending fees to process...");

        // Exemplo: processar uma tarifa fictícia
        // await feeService.ProcessTransferFeeAsync("account123", 100.00m, DateTime.UtcNow);
    }
}