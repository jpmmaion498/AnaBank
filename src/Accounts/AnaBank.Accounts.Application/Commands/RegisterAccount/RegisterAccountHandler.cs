using MediatR;
using Microsoft.Extensions.Logging;
using AnaBank.Accounts.Domain.Entities;
using AnaBank.Accounts.Domain.Interfaces;
using AnaBank.Accounts.Domain.ValueObjects;
using System.Security.Cryptography;

namespace AnaBank.Accounts.Application.Commands.RegisterAccount;

public class RegisterAccountHandler : IRequestHandler<RegisterAccountCommand, RegisterAccountResult>
{
    private readonly IAccountRepository _accountRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ILogger<RegisterAccountHandler> _logger;

    public RegisterAccountHandler(
        IAccountRepository accountRepository, 
        IPasswordHasher passwordHasher,
        ILogger<RegisterAccountHandler> logger)
    {
        _accountRepository = accountRepository ?? throw new ArgumentNullException(nameof(accountRepository));
        _passwordHasher = passwordHasher ?? throw new ArgumentNullException(nameof(passwordHasher));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<RegisterAccountResult> Handle(RegisterAccountCommand request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Iniciando registro de conta para CPF: {Cpf}", request.Cpf);

            var cpf = new Cpf(request.Cpf);

            var existingAccount = await _accountRepository.GetByCpfAsync(cpf);
            if (existingAccount != null)
            {
                _logger.LogWarning("CPF já cadastrado: {Cpf}", cpf.Value);
                throw new InvalidOperationException("CPF já cadastrado");
            }

            var accountNumber = await GenerateUniqueAccountNumber();
            var salt = GenerateSalt();
            var passwordHash = _passwordHasher.HashPassword(request.Password, salt);

            var account = new CurrentAccount(request.Name, cpf, accountNumber, passwordHash, salt);
            var id = await _accountRepository.CreateAsync(account);
            
            _logger.LogInformation("Conta criada com sucesso - ID: {Id}, Número: {Number}", id, accountNumber);

            return new RegisterAccountResult(id, accountNumber.ToString());
        }
        catch (ArgumentException ex) when (ex.Message.Contains("CPF inválido"))
        {
            _logger.LogError(ex, "CPF inválido fornecido: {Cpf}", request.Cpf);
            throw new InvalidOperationException("INVALID_DOCUMENT");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao registrar conta para CPF: {Cpf}", request.Cpf);
            throw;
        }
    }

    private async Task<int> GenerateUniqueAccountNumber()
    {
        var random = new Random();
        int accountNumber;
        
        do
        {
            accountNumber = random.Next(10000000, 99999999);
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