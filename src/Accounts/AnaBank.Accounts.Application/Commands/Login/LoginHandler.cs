using MediatR;
using AnaBank.Accounts.Domain.Interfaces;
using AnaBank.Accounts.Domain.ValueObjects;
using AnaBank.BuildingBlocks.Web.Authentication;
using AnaBank.Accounts.Application.Commands.RegisterAccount;

namespace AnaBank.Accounts.Application.Commands.Login;

public class LoginHandler : IRequestHandler<LoginCommand, LoginResult>
{
    private readonly IAccountRepository _accountRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtTokenService _jwtTokenService;

    public LoginHandler(
        IAccountRepository accountRepository, 
        IPasswordHasher passwordHasher,
        IJwtTokenService jwtTokenService)
    {
        _accountRepository = accountRepository ?? throw new ArgumentNullException(nameof(accountRepository));
        _passwordHasher = passwordHasher ?? throw new ArgumentNullException(nameof(passwordHasher));
        _jwtTokenService = jwtTokenService ?? throw new ArgumentNullException(nameof(jwtTokenService));
    }

    public async Task<LoginResult> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var account = await GetAccountByCpfOrNumber(request.CpfOrNumber);

            if (account == null || !_passwordHasher.VerifyPassword(request.Password, account.PasswordHash, account.Salt))
                throw new UnauthorizedAccessException("USER_UNAUTHORIZED");

            var token = _jwtTokenService.GenerateToken(account.Id, account.Number.ToString());
            return new LoginResult(token);
        }
        catch (ArgumentException)
        {
            throw new UnauthorizedAccessException("USER_UNAUTHORIZED");
        }
    }

    private async Task<Domain.Entities.CurrentAccount?> GetAccountByCpfOrNumber(string cpfOrNumber)
    {
        // Tenta primeiro como CPF
        try
        {
            var cpf = new Cpf(cpfOrNumber);
            return await _accountRepository.GetByCpfAsync(cpf);
        }
        catch
        {
            // Se não for CPF válido, tenta como número da conta
            if (int.TryParse(cpfOrNumber, out var number))
            {
                return await _accountRepository.GetByNumberAsync(number);
            }
        }

        return null;
    }
}