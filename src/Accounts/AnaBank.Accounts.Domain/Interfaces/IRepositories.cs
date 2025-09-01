using AnaBank.Accounts.Domain.Entities;
using AnaBank.Accounts.Domain.ValueObjects;

namespace AnaBank.Accounts.Domain.Interfaces;

public interface IAccountRepository
{
    Task<CurrentAccount?> GetByIdAsync(string id);
    Task<CurrentAccount?> GetByCpfAsync(Cpf cpf);
    Task<CurrentAccount?> GetByNumberAsync(int number);
    Task<string> CreateAsync(CurrentAccount account);
    Task UpdateAsync(CurrentAccount account);
}

public interface IMovementRepository
{
    Task<IEnumerable<Movement>> GetByAccountIdAsync(string accountId);
    Task CreateAsync(Movement movement);
    Task<decimal> GetBalanceAsync(string accountId);
}

public interface IIdempotencyRepository
{
    Task<string?> GetResponseAsync(string key);
    Task SaveAsync(string key, string request, string response);
}