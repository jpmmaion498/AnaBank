using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using FluentValidation;
using AnaBank.Accounts.Infrastructure;
using AnaBank.Accounts.Application.Behaviors;
using AnaBank.BuildingBlocks.Web.Authentication;
using AnaBank.BuildingBlocks.Web.Middleware;
using AnaBank.Accounts.API.Services;
using AnaBank.Accounts.API.BackgroundServices;

var builder = WebApplication.CreateBuilder(args);

var jwtSettings = new JwtSettings
{
    SecretKey = builder.Configuration["Jwt:SecretKey"] ?? "anabank-jwt-secret-key-for-development-only",
    Issuer = builder.Configuration["Jwt:Issuer"] ?? "AnaBank",
    Audience = builder.Configuration["Jwt:Audience"] ?? "AnaBank.APIs",
    ExpirationHours = int.Parse(builder.Configuration["Jwt:ExpirationHours"] ?? "24")
};

builder.Services.AddSingleton(jwtSettings);
builder.Services.AddScoped<IJwtTokenService, JwtTokenService>();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? 
    "Data Source=anabank_accounts.db";

builder.Services.AddAccountsInfrastructure(connectionString);

if (!string.IsNullOrEmpty(builder.Configuration.GetConnectionString("Kafka")))
{
    builder.Services.AddScoped<IFeeConsumerService, FeeConsumerService>();
    builder.Services.AddHostedService<FeeProcessingBackgroundService>();
    Console.WriteLine("Kafka Fee Consumer Service registered");
}
else
{
    Console.WriteLine("Kafka not configured - Fee Consumer Service disabled");
}

builder.Services.AddMediatR(cfg => {
    cfg.RegisterServicesFromAssembly(typeof(AnaBank.Accounts.Application.Commands.RegisterAccount.RegisterAccountCommand).Assembly);
    cfg.AddOpenBehavior(typeof(ValidationBehavior<,>));
});

builder.Services.AddValidatorsFromAssembly(typeof(AnaBank.Accounts.Application.Commands.RegisterAccount.RegisterAccountValidator).Assembly);
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo 
    { 
        Title = "AnaBank Accounts API", 
        Version = "v1",
        Description = "API para gerenciamento de contas correntes do AnaBank"
    });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });

    c.AddSecurityDefinition("Idempotency", new OpenApiSecurityScheme
    {
        Description = "Idempotency key for safe retries. Example: \"Idempotency-Key: 550e8400-e29b-41d4-a716-446655440000\"",
        Name = "Idempotency-Key",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey
    });

    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        c.IncludeXmlComments(xmlPath);
    }
});

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = false;
        options.SaveToken = true;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtSettings.SecretKey)),
            ValidateIssuer = true,
            ValidIssuer = jwtSettings.Issuer,
            ValidateAudience = true,
            ValidAudience = jwtSettings.Audience,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };
    });

builder.Services.AddAuthorization();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "AnaBank Accounts API v1");
    });
}

app.UseCors("AllowAll");

app.UseExceptionHandler(appBuilder =>
{
    appBuilder.Run(async context =>
    {
        context.Response.StatusCode = 500;
        context.Response.ContentType = "application/problem+json";
        
        var problem = new
        {
            type = "INTERNAL_SERVER_ERROR",
            title = "Erro interno do servidor",
            status = 500,
            detail = "Ocorreu um erro interno no servidor"
        };
        
        await context.Response.WriteAsJsonAsync(problem);
    });
});

app.UseIdempotency();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.MapGet("/health", () => Results.Ok(new { Status = "Healthy", Timestamp = DateTime.UtcNow }));

await InitializeDatabase(connectionString);

app.Run();

static async Task InitializeDatabase(string connectionString)
{
    try
    {
        var dbPath = connectionString.Replace("Data Source=", "");
        if (File.Exists(dbPath))
        {
            File.Delete(dbPath);
            Console.WriteLine($"Banco existente removido: {dbPath}");
        }

        var createTablesScript = @"
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
";

        using var connection = new Microsoft.Data.Sqlite.SqliteConnection(connectionString);
        await connection.OpenAsync();
        
        var command = connection.CreateCommand();
        command.CommandText = createTablesScript;
        await command.ExecuteNonQueryAsync();
        
        Console.WriteLine($"Accounts database criado com sucesso: {dbPath}");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error initializing accounts database: {ex.Message}");
        throw;
    }
}

public partial class Program { }
