using FluentValidation;

namespace SMART.ERP.Application.Features.OpportunityDocumentFeature.Commands.CreateOpportunityDocumentCommand
{
    public class CreateOpportunityDocumentCommandValidator : AbstractValidator<CreateOpportunityDocumentCommand>
    {
        public CreateOpportunityDocumentCommandValidator()
        {
            RuleFor(p => p.Name)
                .NotEmpty().WithMessage("{PropertyName} es requerido")
                .MaximumLength(150).WithMessage("{PropertyName} no debe exceder {MaxLength} caracteres");

            RuleFor(p => p.Url)
                .NotEmpty().WithMessage("{PropertyName} es requerido");

            RuleFor(p => p.OpportunityId)
                .NotEmpty().WithMessage("{PropertyName} es requerido");

            RuleFor(p => p.DocumentTypeId)
                .NotEmpty().WithMessage("{PropertyName} es requerido");

            RuleFor(p => p.UserId)
                .NotEmpty().WithMessage("{PropertyName} es requerido");
        }
    }
}
