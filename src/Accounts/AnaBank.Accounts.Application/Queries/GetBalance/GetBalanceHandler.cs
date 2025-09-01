using MediatR;
using AnaBank.Accounts.Domain.Interfaces;

namespace AnaBank.Accounts.Application.Queries.GetBalance;

public class GetBalanceHandler : IRequestHandler<GetBalanceQuery, GetBalanceResult>
{
    private readonly IAccountRepository _accountRepository;
    private readonly IMovementRepository _movementRepository;

    public GetBalanceHandler(IAccountRepository accountRepository, IMovementRepository movementRepository)
    {
        _accountRepository = accountRepository ?? throw new ArgumentNullException(nameof(accountRepository));
        _movementRepository = movementRepository ?? throw new ArgumentNullException(nameof(movementRepository));
    }

    public async Task<GetBalanceResult> Handle(GetBalanceQuery request, CancellationToken cancellationToken)
    {
        var account = await _accountRepository.GetByIdAsync(request.AccountId);
        if (account == null)
            throw new InvalidOperationException("INVALID_ACCOUNT");

        if (!account.IsActive)
            throw new InvalidOperationException("INACTIVE_ACCOUNT");

        var balance = await _movementRepository.GetBalanceAsync(request.AccountId);

        return new GetBalanceResult(
            account.Number.ToString(),
            account.Name,
            DateTime.UtcNow,
            balance
        );
    }
}