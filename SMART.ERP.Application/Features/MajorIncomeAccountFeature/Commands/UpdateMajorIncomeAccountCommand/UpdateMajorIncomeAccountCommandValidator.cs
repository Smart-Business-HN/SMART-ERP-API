using FluentValidation;

namespace SMART.ERP.Application.Features.MajorIncomeAccountFeature.Commands.UpdateMajorIncomeAccountCommand
{
    public class UpdateMajorIncomeAccountCommandValidator : AbstractValidator<UpdateMajorIncomeAccountCommand>
    {
        public UpdateMajorIncomeAccountCommandValidator()
        {
            RuleFor(x => x.Id)
                     .NotEmpty().WithMessage("{PropertyName} es requerido")
                     .NotNull().WithMessage("{PropertyName es requerido}");

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("{PropertyName} es requerido")
                .NotNull().WithMessage("{PropertyName es requerido}")
                .MaximumLength(100).WithMessage("{PropertyName} no debe exceder los {MaxLength} caracteres");
        }
    }
}
