using FluentValidation;

namespace SMART.ERP.Application.Features.ProspectQuoteProductFeature.Commands.CreateProspectQuoteProductCommand
{
    public class CreateProspectQuoteProductCommandValidator : AbstractValidator<CreateProspectQuoteProductCommand>
    {
        public CreateProspectQuoteProductCommandValidator()
        {
            RuleFor(x => x.ProspectId)
                .NotNull().WithMessage("{PropertyName} es requerido")
                .NotEmpty().WithMessage("{PropertyName} es requerido");

            RuleFor(x => x.QuoteProducts)
                .NotEmpty().WithMessage("{PropertyName} debe contener al menos un producto")
                .NotNull().WithMessage("{PropertyName} es requerido");

            RuleForEach(x => x.QuoteProducts).ChildRules(quote =>
            {
                quote.RuleFor(x => x)
                .NotNull().WithMessage("ProductId es requerido")
                .NotEmpty().WithMessage("ProductId es requerido");
            });
        }
    }
}
