using FluentValidation;

namespace SMART.ERP.Application.Features.OpportunityFeature.Commands.CreateOpportunityCommand
{
    public class CreateOpportunityCommandValidator : AbstractValidator<CreateOpportunityCommand>
    {
        public CreateOpportunityCommandValidator()
        {
            RuleFor(p => p.CustomerId)
                .NotNull().WithMessage("{PropertyName} no puede ser nulo")
                .NotEmpty().WithMessage("{PropertyName} no puede estar vacio");

            RuleFor(x => x.OpportunityStepId)
                 .NotEmpty().WithMessage("{PropertyName} es requerido");

            RuleFor(x => x.CustomerId)
                .NotEmpty().WithMessage("{PropertyName} es requerido");

            RuleFor(x => x.CreationDate)
                .NotEmpty().WithMessage("{PropertyName} es requerido");
        }
    }
}
