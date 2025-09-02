using System.Text.Json;
using System.Text;
using Dapper;
using AnaBank.BuildingBlocks.Data;
using Confluent.Kafka;

namespace AnaBank.Fees.Worker.Services;

public interface IFeeService
{
    Task ProcessTransferFeeAsync(string accountId, decimal transferAmount, DateTime transferDate);
}

public class FeeService : IFeeService, IDisposable
{
    private readonly IDbConnectionFactory _connectionFactory;
    private readonly decimal _feeAmount;
    private readonly IConfiguration _configuration;
    private readonly Lazy<IProducer<string, string>> _producer;

    public FeeService(
        IDbConnectionFactory connectionFactory, 
        IConfiguration configuration)
    {
        _connectionFactory = connectionFactory ?? throw new ArgumentNullException(nameof(connectionFactory));
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        _feeAmount = configuration.GetValue<decimal>("FeeSettings:TransferFeeAmount", 2.00m);
        
        // Configurar producer Kafka de forma lazy para evitar problemas de inicialização
        _producer = new Lazy<IProducer<string, string>>(() =>
        {
            var config = new ProducerConfig
            {
                BootstrapServers = configuration.GetConnectionString("Kafka") ?? "localhost:9092"
            };
            return new ProducerBuilder<string, string>(config).Build();
        });
    }

    public async Task ProcessTransferFeeAsync(string accountId, decimal transferAmount, DateTime transferDate)
    {
        try
        {
            // 1. Registrar tarifa no banco local
            var feeId = Guid.NewGuid().ToString();
            await RegisterFeeInDatabase(feeId, accountId, transferDate);
            
            // 2. Publicar evento de tarifa realizada para Kafka (conforme requisito do teste)
            await PublishFeeCompletedEvent(accountId, feeId);
            
            Console.WriteLine($"Fee processed: {feeId} - Account: {accountId} - Amount: {_feeAmount}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error processing fee for account {accountId}: {ex.Message}");
            throw;
        }
    }

    private async Task RegisterFeeInDatabase(string feeId, string accountId, DateTime transferDate)
    {
        const string sql = @"
            INSERT INTO tarifa (idtarifa, idcontacorrente, datamovimento, valor)
            VALUES (@FeeId, @AccountId, @MovementDate, @FeeAmount)";

        using var connection = _connectionFactory.CreateConnection();
        await connection.ExecuteAsync(sql, new
        {
            FeeId = feeId,
            AccountId = accountId,
            MovementDate = transferDate.ToString("dd/MM/yyyy"),
            FeeAmount = _feeAmount
        });
    }

    private async Task PublishFeeCompletedEvent(string accountId, string feeId)
    {
        try
        {
            var feeCompletedEvent = new
            {
                FeeId = feeId,
                AccountId = accountId,
                FeeAmount = _feeAmount,
                ProcessedDate = DateTime.UtcNow
            };

            var message = JsonSerializer.Serialize(feeCompletedEvent);
            
            await _producer.Value.ProduceAsync("fee-completed", new Message<string, string>
            {
                Key = accountId,
                Value = message
            });

            Console.WriteLine($"Fee completed event published for account {accountId}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error publishing fee completed event: {ex.Message}");
            throw;
        }
    }

    public void Dispose()
    {
        if (_producer.IsValueCreated)
        {
            _producer.Value?.Dispose();
        }
    }
}