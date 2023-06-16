using FluentValidation;

namespace SMART.ERP.Application.Features.ProspectQuoteProductFeature.Commands.DeactivateProspectQuoteProductCommand
{
    public class DeactivateProspectQuoteProductCommandValidator : AbstractValidator<DeactivateProspectQuoteProductCommand>
    {
        public DeactivateProspectQuoteProductCommandValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("{PropertyName} es requerido")
                .NotNull().WithMessage("{PropertyName} es requerido");
        }
    }
}
