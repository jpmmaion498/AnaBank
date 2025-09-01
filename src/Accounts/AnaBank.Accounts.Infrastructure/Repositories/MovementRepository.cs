using Dapper;
using AnaBank.Accounts.Domain.Entities;
using AnaBank.Accounts.Domain.Interfaces;
using AnaBank.BuildingBlocks.Data;

namespace AnaBank.Accounts.Infrastructure.Repositories;

public class MovementRepository : IMovementRepository
{
    private readonly IDbConnectionFactory _connectionFactory;

    public MovementRepository(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory ?? throw new ArgumentNullException(nameof(connectionFactory));
    }

    public async Task<IEnumerable<Movement>> GetByAccountIdAsync(string accountId)
    {
        const string sql = @"
            SELECT idmovimento as Id, idcontacorrente as AccountId, tipomovimento as Type, 
                   valor as Value, datamovimento as MovementDate
            FROM movimento 
            WHERE idcontacorrente = @AccountId
            ORDER BY datamovimento DESC";

        using var connection = _connectionFactory.CreateConnection();
        var movements = await connection.QueryAsync<MovementDto>(sql, new { AccountId = accountId });
        
        return movements.Select(m => m.ToEntity());
    }

    public async Task CreateAsync(Movement movement)
    {
        const string sql = @"
            INSERT INTO movimento (idmovimento, idcontacorrente, tipomovimento, valor, datamovimento)
            VALUES (@Id, @AccountId, @Type, @Value, @MovementDate)";

        using var connection = _connectionFactory.CreateConnection();
        await connection.ExecuteAsync(sql, new
        {
            movement.Id,
            movement.AccountId,
            movement.Type,
            movement.Value,
            movement.MovementDate
        });
    }

    public async Task<decimal> GetBalanceAsync(string accountId)
    {
        const string sql = @"
            SELECT 
                COALESCE(SUM(CASE WHEN tipomovimento = 'C' THEN valor ELSE 0 END), 0) -
                COALESCE(SUM(CASE WHEN tipomovimento = 'D' THEN valor ELSE 0 END), 0) as Balance
            FROM movimento 
            WHERE idcontacorrente = @AccountId";

        using var connection = _connectionFactory.CreateConnection();
        var balance = await connection.QuerySingleOrDefaultAsync<decimal>(sql, new { AccountId = accountId });
        
        return balance;
    }

    private class MovementDto
    {
        public string Id { get; set; } = string.Empty;
        public string AccountId { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public decimal Value { get; set; }
        public string MovementDate { get; set; } = string.Empty;

        public Movement ToEntity()
        {
            var movement = new Movement(AccountId, Type, Value);
            
            // Use reflection to set private properties
            var idProperty = typeof(Movement).GetProperty("Id");
            idProperty?.SetValue(movement, Id);
            
            var movementDateProperty = typeof(Movement).GetProperty("MovementDate");
            movementDateProperty?.SetValue(movement, MovementDate);
            
            return movement;
        }
    }
}