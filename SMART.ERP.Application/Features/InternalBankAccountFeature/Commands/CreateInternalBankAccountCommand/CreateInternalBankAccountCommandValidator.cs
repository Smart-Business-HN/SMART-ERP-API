using FluentValidation;

namespace SMART.ERP.Application.Features.InternalBankAccountFeature.Commands.CreateInternalBankAccountCommand
{
    public class CreateInternalBankAccountCommandValidator : AbstractValidator<CreateInternalBankAccountCommand>
    {
        public CreateInternalBankAccountCommandValidator() 
        {
            RuleFor(x => x.Name)
               .NotEmpty().WithMessage("{PropertyName} es requerido")
               .NotNull().WithMessage("{PropertyName} es requerido")
               .MaximumLength(50).WithMessage("{PropertyName} no debe exceder los {MaxLength} caracteres");

            RuleFor(x => x.BankId)
                .NotEmpty().WithMessage("{PropertyName} es requerido")
                .NotNull().WithMessage("{PropertyName} es requerido");

            RuleFor(x => x.IsActive)
                .NotNull().WithMessage("{PropertyName} es requerido");
        }
    }
}
