using FluentValidation;

namespace SMART.ERP.Application.Features.LedgerAccountFeature.Commands.UpdateLedgerAccountCommand
{
    public class UpdateLedgerAccountCommandValidator : AbstractValidator<UpdateLedgerAccountCommand>
    {
        public UpdateLedgerAccountCommandValidator()
        {
            RuleFor(x => x.Id)
                .NotEqual(0).WithMessage("{PropertyName} no puede ser igual a cero");
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("{PropertyName} es requerido")
                .MaximumLength(150).WithMessage("{PropertyName} no debe exceder los {MaxLength} caracteres");
        }
    }
}
