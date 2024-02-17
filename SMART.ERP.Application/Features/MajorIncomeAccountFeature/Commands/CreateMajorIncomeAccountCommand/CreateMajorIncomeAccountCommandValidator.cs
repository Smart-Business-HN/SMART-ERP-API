using FluentValidation;

namespace SMART.ERP.Application.Features.MajorIncomeAccountFeature.Commands.CreateMajorIncomeAccountCommand
{
    public class CreateMajorIncomeAccountCommandValidator : AbstractValidator<CreateMajorIncomeAccountCommand>
    {
        public CreateMajorIncomeAccountCommandValidator() {
            RuleFor(x => x.Name)
                          .NotEmpty().WithMessage("{PropertyName} es requerido")
                          .NotNull().WithMessage("{PropertyName} es requerido")
                          .MaximumLength(100).WithMessage("{PropertyName} no debe exceder los {MaxLength} caracteres");
        }
    }
}
