using FluentValidation;

namespace SMART.ERP.Application.Features.OpportunityStepFeature.Command.CreateOpportunityStepCommand
{
    public class CreateOpportunityStepCommandValidator : AbstractValidator<CreateOpportunityStepCommand>
    {
        public CreateOpportunityStepCommandValidator()
        {
            RuleFor(p => p.Name)
                .NotEmpty().WithMessage("{PropertyName} no puede ser vacio")
                .MaximumLength(50).WithMessage("{PropertyName} no debe exceder {MaxLength} caracteres");
        }
    }
}
