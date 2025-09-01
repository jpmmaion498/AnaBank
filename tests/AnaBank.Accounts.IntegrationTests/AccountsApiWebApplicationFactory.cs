using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Data.Sqlite;
using AnaBank.BuildingBlocks.Data;

namespace AnaBank.Accounts.IntegrationTests;

public class AccountsApiWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            // Remove o serviço de banco original
            var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(IDbConnectionFactory));
            if (descriptor != null)
                services.Remove(descriptor);

            // Adicionar banco de teste em memória
            services.AddSingleton<IDbConnectionFactory>(_ => 
                new SqliteConnectionFactory("Data Source=:memory:"));
        });

        builder.UseEnvironment("Testing");
    }

    public async Task InitializeDatabaseAsync()
    {
        var scopeFactory = Services.GetRequiredService<IServiceScopeFactory>();
        using var scope = scopeFactory.CreateScope();
        var connectionFactory = scope.ServiceProvider.GetRequiredService<IDbConnectionFactory>();

        // Script SQL embutido para testes
        var script = @"
-- Tabela de contas correntes
CREATE TABLE IF NOT EXISTS contacorrente (
    idcontacorrente TEXT(37) PRIMARY KEY,
    numero INTEGER(10) NOT NULL UNIQUE,
    nome TEXT(100) NOT NULL,
    cpf TEXT(11) NOT NULL UNIQUE,
    ativo INTEGER(1) NOT NULL default 1,
    senha TEXT(100) NOT NULL,
    salt TEXT(100) NOT NULL,
    data_criacao DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    CHECK (ativo in (0,1))
);

-- Tabela de movimentos
CREATE TABLE IF NOT EXISTS movimento (
    idmovimento TEXT(37) PRIMARY KEY,
    idcontacorrente TEXT(37) NOT NULL,
    datamovimento TEXT(25) NOT NULL,
    tipomovimento TEXT(1) NOT NULL,
    valor REAL NOT NULL,
    CHECK (tipomovimento in ('C','D')),
    FOREIGN KEY(idcontacorrente) REFERENCES contacorrente(idcontacorrente)
);

-- Tabela de idempotência
CREATE TABLE IF NOT EXISTS idempotencia (
    chave_idempotencia TEXT(37) PRIMARY KEY,
    requisicao TEXT(1000),
    resultado TEXT(1000)
);

-- Índices para performance
CREATE INDEX IF NOT EXISTS idx_movimento_conta ON movimento(idcontacorrente);
CREATE INDEX IF NOT EXISTS idx_movimento_data ON movimento(datamovimento);
CREATE INDEX IF NOT EXISTS idx_contacorrente_cpf ON contacorrente(cpf);
CREATE INDEX IF NOT EXISTS idx_contacorrente_numero ON contacorrente(numero);
";

        // Usar SqliteConnection concreta para métodos assíncronos
        using var connection = connectionFactory.CreateConnection() as SqliteConnection;
        await connection!.OpenAsync();
        
        using var command = connection.CreateCommand();
        command.CommandText = script;
        await command.ExecuteNonQueryAsync();
    }
}