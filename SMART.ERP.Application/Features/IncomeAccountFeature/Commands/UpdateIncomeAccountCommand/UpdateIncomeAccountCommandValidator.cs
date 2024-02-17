using FluentValidation;

namespace SMART.ERP.Application.Features.IncomeAccountFeature.Commands.UpdateIncomeAccountCommand
{
    public class UpdateIncomeAccountCommandValidator : AbstractValidator<UpdateIncomeAccountCommand>
    {
        public UpdateIncomeAccountCommandValidator()
        {
            RuleFor(x => x.Id)
                     .NotEmpty().WithMessage("{PropertyName} es requerido")
                     .NotNull().WithMessage("{PropertyName es requerido}");
            RuleFor(x => x.Name)
                    .NotEmpty().WithMessage("{PropertyName} es requerido")
                    .NotNull().WithMessage("{PropertyName es requerido}")
                .MaximumLength(100).WithMessage("{PropertyName} no debe exceder los {MaxLength} caracteres");
            RuleFor(x => x.AccountNumber)
                   .NotEmpty().WithMessage("{PropertyName} es requerido")
                   .NotNull().WithMessage("{PropertyName es requerido}")
                   .MaximumLength(50).WithMessage("{PropertyName} no debe exceder los {MaxLength} caracteres");
            RuleFor(x => x.MajorIncomeAccountId)
                   .NotNull().WithMessage("{PropertyName} es requerido");
        }
    }
}
