using FluentValidation;

namespace AnaBank.Accounts.Application.Commands.RegisterAccount;

public class RegisterAccountValidator : AbstractValidator<RegisterAccountCommand>
{
    public RegisterAccountValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Nome � obrigat�rio")
            .MaximumLength(100)
            .WithMessage("Nome deve ter no m�ximo 100 caracteres");

        RuleFor(x => x.Cpf)
            .NotEmpty()
            .WithMessage("CPF � obrigat�rio");

        RuleFor(x => x.Password)
            .NotEmpty()
            .WithMessage("Senha � obrigat�ria")
            .MinimumLength(6)
            .WithMessage("Senha deve ter no m�nimo 6 caracteres");
    }
}