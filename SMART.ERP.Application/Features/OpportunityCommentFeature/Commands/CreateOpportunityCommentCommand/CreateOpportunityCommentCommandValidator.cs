using FluentValidation;

namespace SMART.ERP.Application.Features.OpportunityCommentFeature.Commands.CreateOpportunityCommentCommand
{
    public class CreateOpportunityCommentCommandValidator : AbstractValidator<CreateOpportunityCommentCommand>
    {
        public CreateOpportunityCommentCommandValidator()
        {
            RuleFor(p => p.Message)
                .NotEmpty().WithMessage("{PropertyName} es requerido")
                .MaximumLength(600).WithMessage("{PropertyName} no debe exceder {MaxLength} caracteres");

            RuleFor(p => p.OpportunityId)
                .NotEmpty().WithMessage("{PropertyName} es requerido");

            RuleFor(p => p.UserId)
                .NotEmpty().WithMessage("{PropertyName} es requerido");
        }
    }
}
