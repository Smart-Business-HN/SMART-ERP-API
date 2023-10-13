using FluentValidation;

namespace SMART.ERP.Application.Features.BankFeature.Commands.CreateBankCommand
{
    public class CreateBankCommandValidator : AbstractValidator<CreateBankCommand>
    {
        public CreateBankCommandValidator()
        {
            RuleFor(x => x.Name)
                    .NotEmpty().WithMessage("{PropertyName} es requerido")
                    .NotNull().WithMessage("{PropertyName} es requerido")
                    .MaximumLength(50).WithMessage("{PropertyName} no debe exceder los {MaxLength} caracteres");

            RuleFor(x => x.ItIsNationalBank)
                .NotEmpty().WithMessage("{PropertyName} es requerido")
                .NotNull().WithMessage("{PropertyName} es requerido");

            RuleFor(x => x.IsActive)
                .NotNull().WithMessage("{PropertyName} es requerido");
        }
    }
}
