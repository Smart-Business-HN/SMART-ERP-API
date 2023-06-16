using FluentValidation;

namespace SMART.ERP.Application.Features.OpportunityActivityFeature.Commands.DeleteOpportunityActivityCommand
{
    public class DeleteOpportunityActivityCommandValidator : AbstractValidator<DeleteOpportunityActivityCommand>
    {
        public DeleteOpportunityActivityCommandValidator()
        {
            RuleFor(p => p.Id)
                .NotEmpty().WithMessage("{PropertyName} es requerido");
        }
    }
}
