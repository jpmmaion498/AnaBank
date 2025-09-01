using FluentValidation;

namespace AnaBank.Accounts.Application.Commands.MakeMovement;

public class MakeMovementValidator : AbstractValidator<MakeMovementCommand>
{
    public MakeMovementValidator()
    {
        RuleFor(x => x.Type)
            .NotEmpty()
            .WithMessage("Tipo é obrigatório")
            .Must(type => type == "C" || type == "D")
            .WithMessage("Tipo deve ser 'C' (crédito) ou 'D' (débito)");

        RuleFor(x => x.Value)
            .GreaterThan(0)
            .WithMessage("Valor deve ser maior que zero");
    }
}