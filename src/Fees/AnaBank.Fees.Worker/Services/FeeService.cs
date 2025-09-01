using Dapper;
using AnaBank.BuildingBlocks.Data;

namespace AnaBank.Fees.Worker.Services;

public interface IFeeService
{
    Task ProcessTransferFeeAsync(string accountId, decimal transferAmount, DateTime transferDate);
}

public class FeeService : IFeeService
{
    private readonly IDbConnectionFactory _connectionFactory;
    private readonly decimal _feeAmount;

    public FeeService(IDbConnectionFactory connectionFactory, IConfiguration configuration)
    {
        _connectionFactory = connectionFactory ?? throw new ArgumentNullException(nameof(connectionFactory));
        _feeAmount = configuration.GetValue<decimal>("FeeSettings:TransferFeeAmount", 2.00m);
    }

    public async Task ProcessTransferFeeAsync(string accountId, decimal transferAmount, DateTime transferDate)
    {
        // Registrar tarifa no banco
        var feeId = Guid.NewGuid().ToString();
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

        Console.WriteLine($"Fee processed: {feeId} - Account: {accountId} - Amount: {_feeAmount}");
    }
}