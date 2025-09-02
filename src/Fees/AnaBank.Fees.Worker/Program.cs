using AnaBank.Fees.Worker.Services;
using AnaBank.BuildingBlocks.Data;

var builder = Host.CreateApplicationBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? 
    "Data Source=anabank_fees_dev.db";

builder.Services.AddSingleton<IDbConnectionFactory>(_ => new SqliteConnectionFactory(connectionString));

builder.Services.AddHttpClient();
builder.Services.AddScoped<IFeeService, FeeService>();

builder.Services.AddScoped<IKafkaConsumerService, KafkaConsumerService>();

builder.Services.AddHostedService<AnaBank.Fees.Worker.FeeProcessingService>();

var host = builder.Build();

await InitializeDatabase(connectionString);

await host.RunAsync();

static async Task InitializeDatabase(string connectionString)
{
    try
    {
        var dbPath = connectionString.Replace("Data Source=", "");
        if (File.Exists(dbPath))
        {
            File.Delete(dbPath);
        }

        var createTablesScript = @"
CREATE TABLE IF NOT EXISTS tarifa (
    idtarifa TEXT(37) PRIMARY KEY,
    idcontacorrente TEXT(37) NOT NULL,
    datamovimento TEXT(25) NOT NULL,
    valor REAL NOT NULL
);

CREATE INDEX IF NOT EXISTS idx_tarifa_conta ON tarifa(idcontacorrente);
CREATE INDEX IF NOT EXISTS idx_tarifa_data ON tarifa(datamovimento);
";

        using var connection = new Microsoft.Data.Sqlite.SqliteConnection(connectionString);
        await connection.OpenAsync();
        
        var command = connection.CreateCommand();
        command.CommandText = createTablesScript;
        await command.ExecuteNonQueryAsync();
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error initializing fees database: {ex.Message}");
        throw;
    }
}
