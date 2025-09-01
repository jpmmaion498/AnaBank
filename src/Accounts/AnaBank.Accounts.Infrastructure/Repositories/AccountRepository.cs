using Dapper;
using AnaBank.Accounts.Domain.Entities;
using AnaBank.Accounts.Domain.Interfaces;
using AnaBank.Accounts.Domain.ValueObjects;
using AnaBank.BuildingBlocks.Data;

namespace AnaBank.Accounts.Infrastructure.Repositories;

public class AccountRepository : IAccountRepository
{
    private readonly IDbConnectionFactory _connectionFactory;

    public AccountRepository(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory ?? throw new ArgumentNullException(nameof(connectionFactory));
    }

    public async Task<CurrentAccount?> GetByIdAsync(string id)
    {
        const string sql = @"
            SELECT idcontacorrente as Id, nome as Name, cpf as Cpf, numero as Number, 
                   senha as PasswordHash, salt as Salt, ativo as IsActive, data_criacao as CreatedAt
            FROM contacorrente 
            WHERE idcontacorrente = @Id";

        using var connection = _connectionFactory.CreateConnection();
        var account = await connection.QueryFirstOrDefaultAsync<AccountDto>(sql, new { Id = id });
        
        return account?.ToEntity();
    }

    public async Task<CurrentAccount?> GetByCpfAsync(Cpf cpf)
    {
        const string sql = @"
            SELECT idcontacorrente as Id, nome as Name, cpf as Cpf, numero as Number, 
                   senha as PasswordHash, salt as Salt, ativo as IsActive, data_criacao as CreatedAt
            FROM contacorrente 
            WHERE cpf = @Cpf";

        using var connection = _connectionFactory.CreateConnection();
        var account = await connection.QueryFirstOrDefaultAsync<AccountDto>(sql, new { Cpf = cpf.Value });
        
        return account?.ToEntity();
    }

    public async Task<CurrentAccount?> GetByNumberAsync(int number)
    {
        const string sql = @"
            SELECT idcontacorrente as Id, nome as Name, cpf as Cpf, numero as Number, 
                   senha as PasswordHash, salt as Salt, ativo as IsActive, data_criacao as CreatedAt
            FROM contacorrente 
            WHERE numero = @Number";

        using var connection = _connectionFactory.CreateConnection();
        var account = await connection.QueryFirstOrDefaultAsync<AccountDto>(sql, new { Number = number });
        
        return account?.ToEntity();
    }

    public async Task<string> CreateAsync(CurrentAccount account)
    {
        const string sql = @"
            INSERT INTO contacorrente (idcontacorrente, nome, cpf, numero, senha, salt, ativo, data_criacao)
            VALUES (@Id, @Name, @Cpf, @Number, @PasswordHash, @Salt, @IsActive, @CreatedAt)";

        using var connection = _connectionFactory.CreateConnection();
        await connection.ExecuteAsync(sql, new
        {
            account.Id,
            account.Name,
            Cpf = account.Cpf.Value,
            account.Number,
            account.PasswordHash,
            account.Salt,
            IsActive = account.IsActive ? 1 : 0,
            account.CreatedAt
        });

        return account.Id;
    }

    public async Task UpdateAsync(CurrentAccount account)
    {
        const string sql = @"
            UPDATE contacorrente 
            SET nome = @Name, cpf = @Cpf, numero = @Number, 
                senha = @PasswordHash, salt = @Salt, ativo = @IsActive
            WHERE idcontacorrente = @Id";

        using var connection = _connectionFactory.CreateConnection();
        await connection.ExecuteAsync(sql, new
        {
            account.Id,
            account.Name,
            Cpf = account.Cpf.Value,
            account.Number,
            account.PasswordHash,
            account.Salt,
            IsActive = account.IsActive ? 1 : 0
        });
    }

    private class AccountDto
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Cpf { get; set; } = string.Empty;
        public int Number { get; set; }
        public string PasswordHash { get; set; } = string.Empty;
        public string Salt { get; set; } = string.Empty;
        public int IsActive { get; set; }
        public DateTime CreatedAt { get; set; }

        public CurrentAccount ToEntity()
        {
            var account = new CurrentAccount(Name, new Cpf(Cpf), Number, PasswordHash, Salt);
            
            // Use reflection to set private properties
            var idProperty = typeof(CurrentAccount).GetProperty("Id");
            idProperty?.SetValue(account, Id);
            
            var createdAtProperty = typeof(CurrentAccount).GetProperty("CreatedAt");
            createdAtProperty?.SetValue(account, CreatedAt);
            
            if (IsActive == 0)
                account.Deactivate();
            
            return account;
        }
    }
}