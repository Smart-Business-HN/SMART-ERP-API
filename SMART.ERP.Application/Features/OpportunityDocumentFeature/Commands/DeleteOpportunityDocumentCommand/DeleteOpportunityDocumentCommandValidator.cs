using FluentValidation;

namespace SMART.ERP.Application.Features.OpportunityDocumentFeature.Commands.DeleteOpportunityDocumentCommand
{
    public class DeleteOpportunityDocumentCommandValidator : AbstractValidator<DeleteOpportunityDocumentCommand>
    {
        public DeleteOpportunityDocumentCommandValidator()
        {
            RuleFor(p => p.Id)
                .NotEmpty().WithMessage("{PropertyName} es requerido");
        }
    }
}
