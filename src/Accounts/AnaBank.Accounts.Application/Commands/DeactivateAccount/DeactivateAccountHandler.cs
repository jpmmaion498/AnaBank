using MediatR;
using AnaBank.Accounts.Domain.Interfaces;
using AnaBank.Accounts.Application.Commands.RegisterAccount;

namespace AnaBank.Accounts.Application.Commands.DeactivateAccount;

public class DeactivateAccountHandler : IRequestHandler<DeactivateAccountCommand>
{
    private readonly IAccountRepository _accountRepository;
    private readonly IPasswordHasher _passwordHasher;

    public DeactivateAccountHandler(IAccountRepository accountRepository, IPasswordHasher passwordHasher)
    {
        _accountRepository = accountRepository ?? throw new ArgumentNullException(nameof(accountRepository));
        _passwordHasher = passwordHasher ?? throw new ArgumentNullException(nameof(passwordHasher));
    }

    public async Task Handle(DeactivateAccountCommand request, CancellationToken cancellationToken)
    {
        var account = await _accountRepository.GetByIdAsync(request.AccountId);
        if (account == null)
            throw new InvalidOperationException("INVALID_ACCOUNT");

        // Validar senha
        if (!_passwordHasher.VerifyPassword(request.Password, account.PasswordHash, account.Salt))
            throw new UnauthorizedAccessException("USER_UNAUTHORIZED");

        account.Deactivate();
        await _accountRepository.UpdateAsync(account);
    }
}