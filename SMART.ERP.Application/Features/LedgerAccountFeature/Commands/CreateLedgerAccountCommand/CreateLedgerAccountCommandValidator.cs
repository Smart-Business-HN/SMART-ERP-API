using FluentValidation;

namespace SMART.ERP.Application.Features.LedgerAccountFeature.Commands.CreateLedgerAccountCommand
{
    public class CreateLedgerAccountCommandValidator : AbstractValidator<CreateLedgerAccountCommand>
    {
        public CreateLedgerAccountCommandValidator()
        {
            RuleFor(x => x.Code)
                .NotEmpty().WithMessage("{PropertyName} es requerido")
                .MaximumLength(20).WithMessage("{PropertyName} no debe exceder los {MaxLength} caracteres")
                .Matches("^[0-9]+$").WithMessage("{PropertyName} solo puede contener dígitos");
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("{PropertyName} es requerido")
                .MaximumLength(150).WithMessage("{PropertyName} no debe exceder los {MaxLength} caracteres");
            RuleFor(x => x.AccountType)
                .IsInEnum().WithMessage("{PropertyName} no es válido");
        }
    }
}
