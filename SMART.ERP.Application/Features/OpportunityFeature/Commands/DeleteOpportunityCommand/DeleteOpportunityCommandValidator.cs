using FluentValidation;

namespace SMART.ERP.Application.Features.OpportunityFeature.Commands.DeleteOpportunityCommand
{
    public class DeleteOpportunityCommandValidator : AbstractValidator<DeleteOpportunityCommand>
    {
        public DeleteOpportunityCommandValidator()
        {
            RuleFor(p => p.Id)
                .NotNull().WithMessage("{PropertyName} no puede ser nulo")
                .NotEqual(0).WithMessage("{PropertyName} no puede ser igual a cero");
        }
    }
}
