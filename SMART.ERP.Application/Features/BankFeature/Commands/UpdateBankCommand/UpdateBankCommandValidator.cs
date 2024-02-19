using FluentValidation;

namespace SMART.ERP.Application.Features.BankFeature.Commands.UpdateBankCommand
{
    public class UpdateBankCommandValidator : AbstractValidator<UpdateBankCommand>
    {
        public UpdateBankCommandValidator() {
            RuleFor(x => x.Id)
               .NotEmpty().WithMessage("{PropertyName} es requerido")
               .NotNull().WithMessage("{PropertyName es requerido}");

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("{PropertyName} es requerido")
                .NotNull().WithMessage("{PropertyName es requerido}")
                .MaximumLength(50).WithMessage("{PropertyName} no debe exceder los {MaxLength} caracteres");

            RuleFor(x => x.ItIsNationalBank)
                .NotNull().WithMessage("{PropertyName es requerido}");

            RuleFor(x => x.IsActive)
                .NotNull().WithMessage("{PropertyName es requerido}");
        }
    }
}
