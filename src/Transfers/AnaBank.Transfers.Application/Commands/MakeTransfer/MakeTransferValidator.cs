using FluentValidation;

namespace AnaBank.Transfers.Application.Commands.MakeTransfer;

public class MakeTransferValidator : AbstractValidator<MakeTransferCommand>
{
    public MakeTransferValidator()
    {
        RuleFor(x => x.DestinationAccountNumber)
            .NotEmpty()
            .WithMessage("N�mero da conta de destino � obrigat�rio");

        RuleFor(x => x.Value)
            .GreaterThan(0)
            .WithMessage("Valor deve ser maior que zero");
    }
}