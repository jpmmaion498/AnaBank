using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using FluentValidation;
using AnaBank.Accounts.Infrastructure;
using AnaBank.Accounts.Application.Behaviors;
using AnaBank.BuildingBlocks.Web.Authentication;
using AnaBank.BuildingBlocks.Web.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Configuração JWT
var jwtSettings = new JwtSettings
{
    SecretKey = builder.Configuration["Jwt:SecretKey"] ?? "sua-chave-secreta-super-segura-com-pelo-menos-32-caracteres",
    Issuer = builder.Configuration["Jwt:Issuer"] ?? "AnaBank",
    Audience = builder.Configuration["Jwt:Audience"] ?? "AnaBank.APIs",
    ExpirationHours = int.Parse(builder.Configuration["Jwt:ExpirationHours"] ?? "24")
};

builder.Services.AddSingleton(jwtSettings);
builder.Services.AddScoped<IJwtTokenService, JwtTokenService>();

// Database
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? 
    "Data Source=anabank_accounts.db";

builder.Services.AddAccountsInfrastructure(connectionString);

// MediatR
builder.Services.AddMediatR(cfg => {
    cfg.RegisterServicesFromAssembly(typeof(AnaBank.Accounts.Application.Commands.RegisterAccount.RegisterAccountCommand).Assembly);
    cfg.AddOpenBehavior(typeof(ValidationBehavior<,>));
});

// FluentValidation
builder.Services.AddValidatorsFromAssembly(typeof(AnaBank.Accounts.Application.Commands.RegisterAccount.RegisterAccountValidator).Assembly);

// Controllers
builder.Services.AddControllers();

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo 
    { 
        Title = "AnaBank Accounts API", 
        Version = "v1",
        Description = "API para gerenciamento de contas correntes do AnaBank"
    });

    // Configuração JWT no Swagger
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

    // Adicionar header de idempotência
    c.AddSecurityDefinition("Idempotency", new OpenApiSecurityScheme
    {
        Description = "Idempotency key for safe retries. Example: \"Idempotency-Key: 550e8400-e29b-41d4-a716-446655440000\"",
        Name = "Idempotency-Key",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey
    });

    // Incluir comentários XML se disponível
    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        c.IncludeXmlComments(xmlPath);
    }
});

// Authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = false; // Para desenvolvimento
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

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// Cache Redis (diferencial) - removido temporariamente
// if (!string.IsNullOrEmpty(builder.Configuration.GetConnectionString("Redis")))
// {
//     builder.Services.AddStackExchangeRedisCache(options =>
//     {
//         options.Configuration = builder.Configuration.GetConnectionString("Redis");
//     });
// }

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "AnaBank Accounts API v1");
        c.RoutePrefix = string.Empty; // Swagger na raiz
    });
}

app.UseCors("AllowAll");

// Middleware para tratamento global de exceções
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

// Middleware de idempotência
app.UseIdempotency();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// Health check
app.MapGet("/health", () => Results.Ok(new { Status = "Healthy", Timestamp = DateTime.UtcNow }));

// Inicializar banco de dados
await InitializeDatabase(connectionString);

app.Run();

static async Task InitializeDatabase(string connectionString)
{
    try
    {
        var scriptPath = Path.Combine(AppContext.BaseDirectory, "../../../Scripts/accounts-sqlite.sql");
        if (!File.Exists(scriptPath))
        {
            scriptPath = "Scripts/accounts-sqlite.sql";
        }

        if (File.Exists(scriptPath))
        {
            var script = await File.ReadAllTextAsync(scriptPath);
            using var connection = new Microsoft.Data.Sqlite.SqliteConnection(connectionString);
            await connection.OpenAsync();
            
            var command = connection.CreateCommand();
            command.CommandText = script;
            await command.ExecuteNonQueryAsync();
            
            Console.WriteLine("Database initialized successfully");
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error initializing database: {ex.Message}");
    }
}

// Tornar a classe Program acessível para testes
public partial class Program { }
