using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using AnaBank.Transfers.Domain.Interfaces;
using AnaBank.Transfers.Infrastructure.Repositories;
using AnaBank.Transfers.Infrastructure.Clients;
using AnaBank.BuildingBlocks.Data;

namespace AnaBank.Transfers.Infrastructure;

public static class InfrastructureExtensions
{
    public static IServiceCollection AddTransfersInfrastructure(this IServiceCollection services, string connectionString)
    {
        services.AddSingleton<IDbConnectionFactory>(_ => new SqliteConnectionFactory(connectionString));
        services.AddScoped<ITransferRepository, TransferRepository>();
        services.AddScoped<IIdempotencyRepository, IdempotencyRepository>();
        
        services.AddHttpClient<IAccountsClient, AccountsClient>();

        return services;
    }
}