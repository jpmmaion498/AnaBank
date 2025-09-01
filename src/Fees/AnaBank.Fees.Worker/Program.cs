using AnaBank.Fees.Worker.Services;
using AnaBank.BuildingBlocks.Data;

var builder = Host.CreateApplicationBuilder(args);

// Database
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? 
    "Data Source=anabank_fees.db";

builder.Services.AddSingleton<IDbConnectionFactory>(_ => new SqliteConnectionFactory(connectionString));
builder.Services.AddScoped<IFeeService, FeeService>();

// Background Service para processar tarifas
builder.Services.AddHostedService<AnaBank.Fees.Worker.FeeProcessingService>();

var host = builder.Build();

// Inicializar banco de dados
await InitializeDatabase(connectionString);

await host.RunAsync();

static async Task InitializeDatabase(string connectionString)
{
    try
    {
        var scriptPath = Path.Combine(AppContext.BaseDirectory, "../../../Scripts/fees-sqlite.sql");
        if (!File.Exists(scriptPath))
        {
            scriptPath = "Scripts/fees-sqlite.sql";
        }

        if (File.Exists(scriptPath))
        {
            var script = await File.ReadAllTextAsync(scriptPath);
            using var connection = new Microsoft.Data.Sqlite.SqliteConnection(connectionString);
            await connection.OpenAsync();
            
            var command = connection.CreateCommand();
            command.CommandText = script;
            await command.ExecuteNonQueryAsync();
            
            Console.WriteLine("Fees database initialized successfully");
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error initializing fees database: {ex.Message}");
    }
}
