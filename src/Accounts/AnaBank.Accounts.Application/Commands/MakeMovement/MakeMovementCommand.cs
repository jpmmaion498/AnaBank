using MediatR;

namespace AnaBank.Accounts.Application.Commands.MakeMovement;

public record MakeMovementCommand(string? AccountNumber, string Type, decimal Value, string AccountId) : IRequest;