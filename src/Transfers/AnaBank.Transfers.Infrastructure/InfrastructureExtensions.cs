using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using AnaBank.Transfers.Domain.Interfaces;
using AnaBank.Transfers.Infrastructure.Repositories;
using AnaBank.Transfers.Infrastructure.Clients;
using AnaBank.Transfers.Infrastructure.Services;
using AnaBank.BuildingBlocks.Data;

namespace AnaBank.Transfers.Infrastructure;

public static class InfrastructureExtensions
{
    public static IServiceCollection AddTransfersInfrastructure(this IServiceCollection services, string connectionString)
    {
        services.AddSingleton<IDbConnectionFactory>(_ => new SqliteConnectionFactory(connectionString));
        services.AddScoped<ITransferRepository, TransferRepository>();
        services.AddScoped<IIdempotencyRepository, IdempotencyRepository>();
        
        services.AddHttpClient<IAccountsClient, AccountsClient>(client =>
        {
            client.DefaultRequestHeaders.Add("User-Agent", "AnaBank.Transfers.API/1.0");
        })
        .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler()
        {
            ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true
        });
        
        services.AddScoped<IKafkaProducerService, KafkaProducerService>();

        return services;
    }
}