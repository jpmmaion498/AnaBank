using Microsoft.Extensions.DependencyInjection;
using AnaBank.Accounts.Domain.Interfaces;
using AnaBank.Accounts.Infrastructure.Repositories;
using AnaBank.Accounts.Infrastructure.Services;
using AnaBank.Accounts.Application.Commands.RegisterAccount;
using AnaBank.BuildingBlocks.Data;
using AnaBank.BuildingBlocks.Web.Middleware;

namespace AnaBank.Accounts.Infrastructure;

public static class InfrastructureExtensions
{
    public static IServiceCollection AddAccountsInfrastructure(this IServiceCollection services, string connectionString)
    {
        services.AddSingleton<IDbConnectionFactory>(_ => new SqliteConnectionFactory(connectionString));
        services.AddScoped<IAccountRepository, AccountRepository>();
        services.AddScoped<IMovementRepository, MovementRepository>();
        services.AddScoped<IIdempotencyRepository, IdempotencyRepository>();
        services.AddScoped<IPasswordHasher, PasswordHasher>();
        services.AddScoped<IIdempotencyService, IdempotencyService>();

        return services;
    }
}