using Dapper;
using AnaBank.Transfers.Domain.Interfaces;
using AnaBank.BuildingBlocks.Data;

namespace AnaBank.Transfers.Infrastructure.Repositories;

public class IdempotencyRepository : IIdempotencyRepository
{
    private readonly IDbConnectionFactory _connectionFactory;

    public IdempotencyRepository(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory ?? throw new ArgumentNullException(nameof(connectionFactory));
    }

    public async Task<string?> GetResponseAsync(string key)
    {
        const string sql = @"
            SELECT resultado 
            FROM idempotencia 
            WHERE chave_idempotencia = @Key";

        using var connection = _connectionFactory.CreateConnection();
        return await connection.QuerySingleOrDefaultAsync<string>(sql, new { Key = key });
    }

    public async Task SaveAsync(string key, string response)
    {
        const string sql = @"
            INSERT OR REPLACE INTO idempotencia (chave_idempotencia, requisicao, resultado)
            VALUES (@Key, @Request, @Response)";

        using var connection = _connectionFactory.CreateConnection();
        await connection.ExecuteAsync(sql, new
        {
            Key = key,
            Request = "",
            Response = response
        });
    }
}