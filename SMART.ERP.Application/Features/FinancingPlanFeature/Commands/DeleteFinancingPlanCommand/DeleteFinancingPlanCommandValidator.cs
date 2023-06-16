using FluentValidation;

namespace SMART.ERP.Application.Features.FinancingPlanFeature.Commands.DeleteFinancingPlanCommand
{
    public class DeleteFinancingPlanCommandValidator : AbstractValidator<DeleteFinancingPlanCommand>
    {
        public DeleteFinancingPlanCommandValidator()
        {
            RuleFor(p => p.Id)
                .NotNull().WithMessage("{PropertyName} no puede ser nulo")
                .NotEqual(0).WithMessage("{PropertyName} no puede ser igual a cero");
        }
    }
}
