using MediatR;

namespace AnaBank.Accounts.Application.Commands.Login;

public record LoginCommand(string CpfOrNumber, string Password) : IRequest<LoginResult>;

public record LoginResult(string Token);