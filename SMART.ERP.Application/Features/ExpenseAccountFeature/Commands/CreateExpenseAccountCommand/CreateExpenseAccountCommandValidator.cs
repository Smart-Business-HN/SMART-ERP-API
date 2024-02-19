using FluentValidation;

namespace SMART.ERP.Application.Features.ExpenseAccountFeature.Commands.CreateExpenseAccountCommand
{
    public class CreateExpenseAccountCommandValidator : AbstractValidator<CreateExpenseAccountCommand>
    {
        public CreateExpenseAccountCommandValidator()
        {
            RuleFor(x => x.Name)
              .NotEmpty().WithMessage("{PropertyName} es requerido")
              .NotNull().WithMessage("{PropertyName} es requerido")
              .MaximumLength(100).WithMessage("{PropertyName} no debe exceder los {MaxLength} caracteres");
            RuleFor(x => x.AccountNumber)
             .NotEmpty().WithMessage("{PropertyName} es requerido")
             .NotNull().WithMessage("{PropertyName} es requerido")
             .MaximumLength(100).WithMessage("{PropertyName} no debe exceder los {MaxLength} caracteres");
            RuleFor(x => x.MajorExpenseAccountId)
              .NotNull().WithMessage("{PropertyName} es requerido");
        }
    }
}
