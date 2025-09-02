using AnaBank.Fees.Worker.Services;
using AnaBank.BuildingBlocks.Data;

var builder = Host.CreateApplicationBuilder(args);

// Database
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? 
    "Data Source=anabank_fees_dev.db";

builder.Services.AddSingleton<IDbConnectionFactory>(_ => new SqliteConnectionFactory(connectionString));

// HTTP Client para comunicação com APIs
builder.Services.AddHttpClient();
builder.Services.AddScoped<IFeeService, FeeService>();

// Kafka Consumer Service
builder.Services.AddScoped<IKafkaConsumerService, KafkaConsumerService>();

// Background Service para processar tarifas via Kafka
builder.Services.AddHostedService<AnaBank.Fees.Worker.FeeProcessingService>();

var host = builder.Build();

// Inicializar banco de dados
await InitializeDatabase(connectionString);

await host.RunAsync();

static async Task InitializeDatabase(string connectionString)
{
    try
    {
        // SEMPRE remover o banco existente para garantir reset completo
        var dbPath = connectionString.Replace("Data Source=", "");
        if (File.Exists(dbPath))
        {
            File.Delete(dbPath);
            Console.WriteLine($"Banco fees existente removido: {dbPath}");
        }

        // Criar o script SQL inline
        var createTablesScript = @"
-- Tabela de tarifas
CREATE TABLE IF NOT EXISTS tarifa (
    idtarifa TEXT(37) PRIMARY KEY,
    idcontacorrente TEXT(37) NOT NULL,
    datamovimento TEXT(25) NOT NULL,
    valor REAL NOT NULL
);

-- Índices para performance
CREATE INDEX IF NOT EXISTS idx_tarifa_conta ON tarifa(idcontacorrente);
CREATE INDEX IF NOT EXISTS idx_tarifa_data ON tarifa(datamovimento);
";

        using var connection = new Microsoft.Data.Sqlite.SqliteConnection(connectionString);
        await connection.OpenAsync();
        
        var command = connection.CreateCommand();
        command.CommandText = createTablesScript;
        await command.ExecuteNonQueryAsync();
        
        Console.WriteLine($"Fees database criado com sucesso: {dbPath}");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error initializing fees database: {ex.Message}");
        throw;
    }
}
