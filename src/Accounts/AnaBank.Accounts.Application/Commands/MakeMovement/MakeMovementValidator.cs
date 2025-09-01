using FluentValidation;

namespace AnaBank.Accounts.Application.Commands.MakeMovement;

public class MakeMovementValidator : AbstractValidator<MakeMovementCommand>
{
    public MakeMovementValidator()
    {
        RuleFor(x => x.Type)
            .NotEmpty()
            .WithMessage("Tipo � obrigat�rio")
            .Must(type => type == "C" || type == "D")
            .WithMessage("Tipo deve ser 'C' (cr�dito) ou 'D' (d�bito)");

        RuleFor(x => x.Value)
            .GreaterThan(0)
            .WithMessage("Valor deve ser maior que zero");
    }
}