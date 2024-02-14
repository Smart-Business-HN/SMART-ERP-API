using FluentValidation;

namespace SMART.ERP.Application.Features.MajorExpenseAccountFeature.Commands.CreateMajorExpenseAccountCommand
{
    public class CreateMajorExpenseAccountCommandValidator : AbstractValidator<CreateMajorExpenseAccountCommand>
    {
        public CreateMajorExpenseAccountCommandValidator() {
            RuleFor(x => x.Name)
                       .NotEmpty().WithMessage("{PropertyName} es requerido")
                       .NotNull().WithMessage("{PropertyName} es requerido")
                       .MaximumLength(100).WithMessage("{PropertyName} no debe exceder los {MaxLength} caracteres");
        }
    }
}
