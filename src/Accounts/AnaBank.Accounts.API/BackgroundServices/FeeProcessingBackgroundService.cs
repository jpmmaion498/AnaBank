using AnaBank.Accounts.API.Services;

namespace AnaBank.Accounts.API.BackgroundServices;

public class FeeProcessingBackgroundService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<FeeProcessingBackgroundService> _logger;

    public FeeProcessingBackgroundService(
        IServiceProvider serviceProvider,
        ILogger<FeeProcessingBackgroundService> logger)
    {
        _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // Aguardar um pouco para a aplicação web inicializar completamente
        await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
        
        _logger.LogInformation("Fee Processing Background Service started (delayed startup)");

        try
        {
            using var scope = _serviceProvider.CreateScope();
            var feeConsumerService = scope.ServiceProvider.GetRequiredService<IFeeConsumerService>();
            
            await feeConsumerService.StartConsumingAsync(stoppingToken);
        }
        catch (OperationCanceledException)
        {
            _logger.LogInformation("Fee Processing Background Service cancelled");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in Fee Processing Background Service - service will continue running");
            // Não quebrar a aplicação, apenas logar o erro
        }
        finally
        {
            _logger.LogInformation("Fee Processing Background Service stopped");
        }
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Fee Processing Background Service stopping...");
        await base.StopAsync(cancellationToken);
    }
}