using MediatR;
using AnaBank.Accounts.Domain.Entities;
using AnaBank.Accounts.Domain.Interfaces;
using AnaBank.Accounts.Domain.ValueObjects;
using System.Security.Cryptography;
using System.Text;

namespace AnaBank.Accounts.Application.Commands.RegisterAccount;

public class RegisterAccountHandler : IRequestHandler<RegisterAccountCommand, RegisterAccountResult>
{
    private readonly IAccountRepository _accountRepository;
    private readonly IPasswordHasher _passwordHasher;

    public RegisterAccountHandler(IAccountRepository accountRepository, IPasswordHasher passwordHasher)
    {
        _accountRepository = accountRepository ?? throw new ArgumentNullException(nameof(accountRepository));
        _passwordHasher = passwordHasher ?? throw new ArgumentNullException(nameof(passwordHasher));
    }

    public async Task<RegisterAccountResult> Handle(RegisterAccountCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var cpf = new Cpf(request.Cpf);

            // Verifica se CPF já existe
            var existingAccount = await _accountRepository.GetByCpfAsync(cpf);
            if (existingAccount != null)
                throw new InvalidOperationException("CPF já cadastrado");

            // Gera número da conta único
            var accountNumber = await GenerateUniqueAccountNumber();

            // Gera salt e hash da senha
            var salt = GenerateSalt();
            var passwordHash = _passwordHasher.HashPassword(request.Password, salt);

            // Cria a conta
            var account = new CurrentAccount(request.Name, cpf, accountNumber, passwordHash, salt);
            var id = await _accountRepository.CreateAsync(account);

            return new RegisterAccountResult(id, accountNumber.ToString());
        }
        catch (ArgumentException ex) when (ex.Message.Contains("CPF inválido"))
        {
            throw new InvalidOperationException("INVALID_DOCUMENT");
        }
    }

    private async Task<int> GenerateUniqueAccountNumber()
    {
        var random = new Random();
        int accountNumber;
        
        do
        {
            accountNumber = random.Next(10000000, 99999999); // 8 dígitos
        }
        while (await _accountRepository.GetByNumberAsync(accountNumber) != null);
        
        return accountNumber;
    }

    private static string GenerateSalt()
    {
        using var rng = RandomNumberGenerator.Create();
        var saltBytes = new byte[32];
        rng.GetBytes(saltBytes);
        return Convert.ToBase64String(saltBytes);
    }
}

public interface IPasswordHasher
{
    string HashPassword(string password, string salt);
    bool VerifyPassword(string password, string hash, string salt);
}