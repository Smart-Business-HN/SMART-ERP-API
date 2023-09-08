using FluentValidation;

namespace SMART.ERP.Application.Features.TaxFeature.Commands.DeleteTaxCommand
{
    public class DeleteTaxCommandValidator : AbstractValidator<DeleteTaxCommand>
    {
        public DeleteTaxCommandValidator() {
            RuleFor(p => p.Id)
                   .NotEmpty().WithMessage("{PropertyName} no puede ser nulo")
                   .NotEqual(0).WithMessage("{PropertyName} no puede ser igual a cero");
        }
    }
}
