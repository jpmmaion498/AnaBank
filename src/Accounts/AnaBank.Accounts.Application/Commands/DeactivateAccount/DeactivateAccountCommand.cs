using MediatR;

namespace AnaBank.Accounts.Application.Commands.DeactivateAccount;

public record DeactivateAccountCommand(string AccountId, string Password) : IRequest;