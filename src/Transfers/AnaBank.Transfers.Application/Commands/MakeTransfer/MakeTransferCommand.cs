using MediatR;

namespace AnaBank.Transfers.Application.Commands.MakeTransfer;

public record MakeTransferCommand(string DestinationAccountNumber, decimal Value, string OriginAccountId) : IRequest;