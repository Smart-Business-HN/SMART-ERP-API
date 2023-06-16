using FluentValidation;

namespace SMART.ERP.Application.Features.OpportunityCommentFeature.Commands.DeleteOpportunityCommentCommand
{
    public class DeleteOpportunityCommentCommandValidator : AbstractValidator<DeleteOpportunityCommentCommand>
    {
        public DeleteOpportunityCommentCommandValidator()
        {
            RuleFor(p => p.Id)
                .NotEmpty().WithMessage("{PropertyName} es requerido");
        }
    }
}
