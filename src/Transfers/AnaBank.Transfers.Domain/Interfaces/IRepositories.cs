using AnaBank.Transfers.Domain.Entities;

namespace AnaBank.Transfers.Domain.Interfaces;

public interface ITransferRepository
{
    Task<Transfer?> GetByIdAsync(string id);
    Task<string> CreateAsync(Transfer transfer);
    Task UpdateAsync(Transfer transfer);
}

public interface IIdempotencyRepository
{
    Task<string?> GetResponseAsync(string key);
    Task SaveAsync(string key, string response);
}

public interface IAccountsClient
{
    Task DebitAsync(string accountId, decimal value, string authToken);
    Task CreditAsync(string accountNumber, decimal value, string authToken);
}