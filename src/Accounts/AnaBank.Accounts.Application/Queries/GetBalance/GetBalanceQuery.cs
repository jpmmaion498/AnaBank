using MediatR;

namespace AnaBank.Accounts.Application.Queries.GetBalance;

public record GetBalanceQuery(string AccountId) : IRequest<GetBalanceResult>;

public record GetBalanceResult(string Number, string Name, DateTime DateTime, decimal Balance);