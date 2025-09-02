using MediatR;
using AnaBank.Accounts.Domain.Entities;
using AnaBank.Accounts.Domain.Interfaces;

namespace AnaBank.Accounts.Application.Commands.MakeMovement;

public class MakeMovementHandler : IRequestHandler<MakeMovementCommand>
{
    private readonly IAccountRepository _accountRepository;
    private readonly IMovementRepository _movementRepository;

    public MakeMovementHandler(IAccountRepository accountRepository, IMovementRepository movementRepository)
    {
        _accountRepository = accountRepository ?? throw new ArgumentNullException(nameof(accountRepository));
        _movementRepository = movementRepository ?? throw new ArgumentNullException(nameof(movementRepository));
    }

    public async Task Handle(MakeMovementCommand request, CancellationToken cancellationToken)
    {
        string targetAccountId;
        
        if (!string.IsNullOrEmpty(request.AccountNumber))
        {
            if (request.Type != "C")
                throw new InvalidOperationException("INVALID_TYPE");
                
            if (int.TryParse(request.AccountNumber, out var accountNumber))
            {
                var targetAccount = await _accountRepository.GetByNumberAsync(accountNumber);
                if (targetAccount == null)
                    throw new InvalidOperationException("INVALID_ACCOUNT");
                
                targetAccountId = targetAccount.Id;
            }
            else
            {
                throw new InvalidOperationException("INVALID_ACCOUNT");
            }
        }
        else
        {
            targetAccountId = request.AccountId;
        }

        var account = await _accountRepository.GetByIdAsync(targetAccountId);
        if (account == null)
            throw new InvalidOperationException("INVALID_ACCOUNT");

        if (!account.IsActive)
            throw new InvalidOperationException("INACTIVE_ACCOUNT");

        if (request.Value <= 0)
            throw new InvalidOperationException("INVALID_VALUE");

        if (request.Type != "C" && request.Type != "D")
            throw new InvalidOperationException("INVALID_TYPE");

        if (request.Type == "D" && targetAccountId == request.AccountId)
        {
            var currentBalance = await _movementRepository.GetBalanceAsync(targetAccountId);
            if (currentBalance < request.Value)
                throw new InvalidOperationException("INSUFFICIENT_FUNDS");
        }

        var movement = new Movement(targetAccountId, request.Type, request.Value);
        await _movementRepository.CreateAsync(movement);
    }
}