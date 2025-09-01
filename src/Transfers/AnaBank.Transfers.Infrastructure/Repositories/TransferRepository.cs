using Dapper;
using AnaBank.Transfers.Domain.Entities;
using AnaBank.Transfers.Domain.Interfaces;
using AnaBank.BuildingBlocks.Data;

namespace AnaBank.Transfers.Infrastructure.Repositories;

public class TransferRepository : ITransferRepository
{
    private readonly IDbConnectionFactory _connectionFactory;

    public TransferRepository(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory ?? throw new ArgumentNullException(nameof(connectionFactory));
    }

    public async Task<Transfer?> GetByIdAsync(string id)
    {
        const string sql = @"
            SELECT idtransferencia as Id, idcontacorrente_origem as OriginAccountId, 
                   idcontacorrente_destino as DestinationAccountId, valor as Value, 
                   datamovimento as MovementDate
            FROM transferencia 
            WHERE idtransferencia = @Id";

        using var connection = _connectionFactory.CreateConnection();
        var transfer = await connection.QueryFirstOrDefaultAsync<TransferDto>(sql, new { Id = id });
        
        return transfer?.ToEntity();
    }

    public async Task<string> CreateAsync(Transfer transfer)
    {
        const string sql = @"
            INSERT INTO transferencia (idtransferencia, idcontacorrente_origem, idcontacorrente_destino, datamovimento, valor)
            VALUES (@Id, @OriginAccountId, @DestinationAccountId, @MovementDate, @Value)";

        using var connection = _connectionFactory.CreateConnection();
        await connection.ExecuteAsync(sql, new
        {
            transfer.Id,
            transfer.OriginAccountId,
            transfer.DestinationAccountId,
            MovementDate = transfer.CreatedAt.ToString("dd/MM/yyyy"),
            transfer.Value
        });

        return transfer.Id;
    }

    public async Task UpdateAsync(Transfer transfer)
    {
        const string sql = @"
            UPDATE transferencia 
            SET idcontacorrente_origem = @OriginAccountId, idcontacorrente_destino = @DestinationAccountId,
                valor = @Value, datamovimento = @MovementDate
            WHERE idtransferencia = @Id";

        using var connection = _connectionFactory.CreateConnection();
        await connection.ExecuteAsync(sql, new
        {
            transfer.Id,
            transfer.OriginAccountId,
            transfer.DestinationAccountId,
            MovementDate = transfer.CreatedAt.ToString("dd/MM/yyyy"),
            transfer.Value
        });
    }

    private class TransferDto
    {
        public string Id { get; set; } = string.Empty;
        public string OriginAccountId { get; set; } = string.Empty;
        public string DestinationAccountId { get; set; } = string.Empty;
        public decimal Value { get; set; }
        public string MovementDate { get; set; } = string.Empty;

        public Transfer ToEntity()
        {
            var transfer = new Transfer(OriginAccountId, DestinationAccountId, Value);
            
            // Use reflection to set private properties if needed
            var idProperty = typeof(Transfer).GetProperty("Id");
            idProperty?.SetValue(transfer, Id);
            
            return transfer;
        }
    }
}