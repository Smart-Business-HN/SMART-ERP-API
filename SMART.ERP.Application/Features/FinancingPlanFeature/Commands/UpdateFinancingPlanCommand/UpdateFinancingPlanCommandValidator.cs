using FluentValidation;

namespace SMART.ERP.Application.Features.FinancingPlanFeature.Commands.UpdateFinancingPlanCommand
{
    public class UpdateFinancingPlanCommandValidator : AbstractValidator<UpdateFinancingPlanCommand>
    {
        public UpdateFinancingPlanCommandValidator()
        {
            RuleFor(x => x.Id)
                .NotNull().WithMessage("{PropertyName} no puede ser nulo")
                .NotEqual(0).WithMessage("{PropertyName} no puede ser igual a cero");

            RuleFor(p => p.Name)
                .NotEmpty().WithMessage("{PropertyName} no puede ser vacio")
                .MaximumLength(50).WithMessage("{PropertyName} no debe exceder {MaxLength} caracteres");

            RuleFor(p => p.Description)
                .NotEmpty().WithMessage("{PropertyName} no puede ser vacio")
                .MaximumLength(600).WithMessage("{PropertyName} no debe exceder {MaxLength} caracteres");

            RuleFor(p => p.ProviderId)
                .NotNull().WithMessage("{PropertyName} no puede ser nulo")
                .NotEqual(0).WithMessage("{PropertyName} no puede ser igual a cero");
        }
    }
}
