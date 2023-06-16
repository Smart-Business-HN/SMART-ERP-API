using FluentValidation;

namespace SMART.ERP.Application.Features.OpportunityStepFeature.Command.DeleteOpportunityStepCommand
{
    public class DeleteOpportunityStepCommandValidator : AbstractValidator<DeleteOpportunityStepCommand>
    {
        public DeleteOpportunityStepCommandValidator()
        {
            RuleFor(p => p.Id)
                .NotEmpty().WithMessage("{PropertyName} no puede ser nulo")
                .NotEqual(0).WithMessage("{PropertyName} no puede ser igual a cero");
        }
    }
}
