using FluentValidation;

namespace AnaBank.Transfers.Application.Commands.MakeTransfer;

public class MakeTransferValidator : AbstractValidator<MakeTransferCommand>
{
    public MakeTransferValidator()
    {
        RuleFor(x => x.DestinationAccountNumber)
            .NotEmpty()
            .WithMessage("Número da conta de destino é obrigatório");

        RuleFor(x => x.Value)
            .GreaterThan(0)
            .WithMessage("Valor deve ser maior que zero");
    }
}