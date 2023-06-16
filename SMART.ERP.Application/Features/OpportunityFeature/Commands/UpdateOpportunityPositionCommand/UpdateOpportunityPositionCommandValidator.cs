using FluentValidation;

namespace SMART.ERP.Application.Features.OpportunityFeature.Commands.UpdateOpportunityPositionCommand
{
    public class UpdateOpportunityPositionCommandValidator : AbstractValidator<UpdateOpportunityPositionCommand>
    {
        public UpdateOpportunityPositionCommandValidator()
        {
            RuleFor(x => x.OpportunityId)
                .NotEmpty().WithMessage("{PropertyName} es requerido");
            RuleFor(x => x.OpportunityStepId)
                .NotEmpty().WithMessage("{PropertyName} es requerido");
            RuleFor(x => x.Position)
                .NotEmpty().WithMessage("{PropertyName} es requerido");
        }
    }
}
