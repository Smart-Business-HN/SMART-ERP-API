using FluentValidation;

namespace SMART.ERP.Application.Features.OpportunityCommentFeature.Commands.UpdateOpportunityCommentCommand
{
    public class UpdateOpportunityCommentCommandValidator : AbstractValidator<UpdateOpportunityCommentCommand>
    {
        public UpdateOpportunityCommentCommandValidator()
        {
            RuleFor(p => p.Id)
                .NotEmpty().WithMessage("{PropertyName} es requerido");

            RuleFor(p => p.Message)
                .NotEmpty().WithMessage("{PropertyName} es requerido")
                .MaximumLength(200).WithMessage("{PropertyName} no debe exceder {MaxLength} caracteres");

            RuleFor(p => p.OpportunityId)
                .NotEmpty().WithMessage("{PropertyName} es requerido");

            RuleFor(p => p.UserId)
                .NotEmpty().WithMessage("{PropertyName} es requerido");
        }
    }
}
