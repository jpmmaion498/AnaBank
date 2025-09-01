using MediatR;

namespace AnaBank.Accounts.Application.Commands.RegisterAccount;

public record RegisterAccountCommand(string Name, string Cpf, string Password) : IRequest<RegisterAccountResult>;

public record RegisterAccountResult(string Id, string Number);