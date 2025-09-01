using AnaBank.Accounts.Domain.Interfaces;
using AnaBank.BuildingBlocks.Web.Middleware;

namespace AnaBank.Accounts.Infrastructure.Services;

public class IdempotencyService : IIdempotencyService
{
    private readonly IIdempotencyRepository _repository;

    public IdempotencyService(IIdempotencyRepository repository)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
    }

    public async Task<string?> GetResponseAsync(string key)
    {
        return await _repository.GetResponseAsync(key);
    }

    public async Task SaveResponseAsync(string key, string response)
    {
        await _repository.SaveAsync(key, "", response);
    }
}