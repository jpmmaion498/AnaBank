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
        // Determinar a conta de destino
        string targetAccountId;
        
        if (!string.IsNullOrEmpty(request.AccountNumber))
        {
            // Se informou n�mero da conta, validar se � diferente da conta logada e se � cr�dito
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
            // Usar a conta do usu�rio logado
            targetAccountId = request.AccountId;
        }

        // Validar se a conta existe
        var account = await _accountRepository.GetByIdAsync(targetAccountId);
        if (account == null)
            throw new InvalidOperationException("INVALID_ACCOUNT");

        // Validar se a conta est� ativa
        if (!account.IsActive)
            throw new InvalidOperationException("INACTIVE_ACCOUNT");

        // Validar valor
        if (request.Value <= 0)
            throw new InvalidOperationException("INVALID_VALUE");

        // Validar tipo
        if (request.Type != "C" && request.Type != "D")
            throw new InvalidOperationException("INVALID_TYPE");

        // Se for d�bito na pr�pria conta, verificar saldo
        if (request.Type == "D" && targetAccountId == request.AccountId)
        {
            var currentBalance = await _movementRepository.GetBalanceAsync(targetAccountId);
            if (currentBalance < request.Value)
                throw new InvalidOperationException("INSUFFICIENT_FUNDS");
        }

        // Criar movimenta��o
        var movement = new Movement(targetAccountId, request.Type, request.Value);
        await _movementRepository.CreateAsync(movement);
    }
}