using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Data.Sqlite;
using AnaBank.BuildingBlocks.Data;
using Microsoft.Extensions.Configuration;
using AnaBank.Accounts.API.Services;
using AnaBank.Accounts.API.BackgroundServices;

namespace AnaBank.Accounts.IntegrationTests;

public class AccountsApiWebApplicationFactory : WebApplicationFactory<Program>
{
    private readonly SqliteConnection _connection;

    public AccountsApiWebApplicationFactory()
    {
        _connection = new SqliteConnection("Data Source=test_db_" + Guid.NewGuid().ToString("N")[..8] + ".db");
        _connection.Open();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            var dbDescriptor = services.SingleOrDefault(d => d.ServiceType == typeof(IDbConnectionFactory));
            if (dbDescriptor != null)
                services.Remove(dbDescriptor);

            var bgServiceDescriptor = services.SingleOrDefault(d => d.ServiceType == typeof(FeeProcessingBackgroundService));
            if (bgServiceDescriptor != null)
                services.Remove(bgServiceDescriptor);

            var feeConsumerDescriptor = services.SingleOrDefault(d => d.ServiceType == typeof(IFeeConsumerService));
            if (feeConsumerDescriptor != null)
                services.Remove(feeConsumerDescriptor);

            services.AddSingleton<IDbConnectionFactory>(_ => 
                new SqliteConnectionFactory(_connection.ConnectionString));
        });

        builder.UseEnvironment("Testing");
        
        builder.ConfigureAppConfiguration((context, config) =>
        {
            config.AddInMemoryCollection(new[]
            {
                new KeyValuePair<string, string?>("ConnectionStrings:Kafka", ""),
                new KeyValuePair<string, string?>("Jwt:SecretKey", "test-key-for-integration-tests-with-at-least-32-chars"),
                new KeyValuePair<string, string?>("Jwt:Issuer", "AnaBank.Test"),
                new KeyValuePair<string, string?>("Jwt:Audience", "AnaBank.Test"),
                new KeyValuePair<string, string?>("Jwt:ExpirationHours", "1")
            });
        });
    }

    public async Task InitializeDatabaseAsync()
    {
        var script = @"
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

CREATE TABLE IF NOT EXISTS movimento (
    idmovimento TEXT(37) PRIMARY KEY,
    idcontacorrente TEXT(37) NOT NULL,
    datamovimento TEXT(25) NOT NULL,
    tipomovimento TEXT(1) NOT NULL,
    valor REAL NOT NULL,
    CHECK (tipomovimento in ('C','D')),
    FOREIGN KEY(idcontacorrente) REFERENCES contacorrente(idcontacorrente)
);

CREATE TABLE IF NOT EXISTS idempotencia (
    chave_idempotencia TEXT(37) PRIMARY KEY,
    requisicao TEXT(1000),
    resultado TEXT(1000)
);

CREATE INDEX IF NOT EXISTS idx_movimento_conta ON movimento(idcontacorrente);
CREATE INDEX IF NOT EXISTS idx_movimento_data ON movimento(datamovimento);
CREATE INDEX IF NOT EXISTS idx_contacorrente_cpf ON contacorrente(cpf);
CREATE INDEX IF NOT EXISTS idx_contacorrente_numero ON contacorrente(numero);

DELETE FROM contacorrente;
DELETE FROM movimento;
DELETE FROM idempotencia;
";

        using var command = _connection.CreateCommand();
        command.CommandText = script;
        await command.ExecuteNonQueryAsync();
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _connection?.Dispose();
        }
        base.Dispose(disposing);
    }
}