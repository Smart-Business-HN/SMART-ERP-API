using FluentValidation;

namespace SMART.ERP.Application.Features.OpportunityFeature.Commands.UpdateOpportunityCommand
{
    public class UpdateOpportunityCommandValidator : AbstractValidator<UpdateOpportunityCommand>
    {
        public UpdateOpportunityCommandValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("{PropertyName} es requerido");

            RuleFor(p => p.CustomerId)
                .NotNull().WithMessage("{PropertyName} no puede ser nulo")
                .NotEmpty().WithMessage("{PropertyName} no puede estar vacio");

            RuleFor(x => x.CreationDate)
                .NotEmpty().WithMessage("{PropertyName} es requerido");

            RuleFor(x => x.Description)
                .MaximumLength(100).WithMessage("{PropertyName} no debe exceder los {MaxLength} caracteres")
                .When(x => x.Description != null);

            RuleFor(x => x.RecommendedBy)
                .MaximumLength(500).WithMessage("{PropertyName} no debe exceder los {MaxLength} caracteres")
                .When(x => x.RecommendedBy != null);

            RuleFor(x => x.OpportunityType)
                .MaximumLength(50).WithMessage("{PropertyName} no debe exceder los {MaxLength} caracteres")
                .When(x => x.OpportunityType != null);

            RuleFor(x => x.ProbabilityPercentage)
                .Must(prob => prob >= 0 && prob <= 100).WithMessage("{PropertyName} debe estar en el rango de 0 a 100");
        }
    }
}
