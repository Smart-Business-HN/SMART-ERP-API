using FluentValidation;

namespace SMART.ERP.Application.Features.QuoteProductFeature.Commands.UpdateQuoteProductCommand
{
    public class UpdateQuoteProductCommandValidator : AbstractValidator<UpdateQuoteProductCommand>
    {
        public UpdateQuoteProductCommandValidator()
        {
            RuleFor(p => p.Id)
                .NotNull().WithMessage("{PropertyName} no puede ser nulo")
                .NotEqual(0).WithMessage("{PropertyName} no puede ser igual a cero");

            RuleFor(p => p.Quantity)
                .NotNull().WithMessage("{PropertyName} no puede ser nulo")
                .NotEqual(0).WithMessage("{PropertyName} no puede ser igual a cero");
        }
    }
}
