using FluentValidation;

namespace SMART.ERP.Application.Features.OpportunityFeature.Commands.CreateOpportunityCartCommand
{
    public class CreateOpportunityCartCommandValidator : AbstractValidator<CreateOpportunityCartCommand>
    {
        public CreateOpportunityCartCommandValidator()
        {
            RuleFor(x => x.WishListId)
                .NotEmpty().WithMessage("{PropertyName} es requerido");
        }
    }
}
