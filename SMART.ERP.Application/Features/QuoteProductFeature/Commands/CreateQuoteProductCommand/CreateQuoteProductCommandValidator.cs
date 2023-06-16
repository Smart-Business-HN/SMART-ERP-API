using FluentValidation;

namespace SMART.ERP.Application.Features.QuoteProductFeature.Commands.CreateQuoteProductCommand
{
    public class CreateQuoteProductCommandValidator : AbstractValidator<CreateQuoteProductCommand>
    {
        public CreateQuoteProductCommandValidator()
        {
            RuleFor(x => x.QuoteProducts)
                .NotEmpty().WithMessage("{PropertyName} no debe ser vacio")
                .NotNull().WithMessage("{PropertyName} es requerido");

            RuleForEach(x => x.QuoteProducts)
                .ChildRules(p => p.RuleFor(p => p.ProductId)
                    .NotNull().WithMessage("{PropertyName} no puede ser nulo")
                    .NotEqual(0).WithMessage("{PropertyName} no puede ser igual a cero"))
                .ChildRules(p => p.RuleFor(p => p.OpportunityId)
                    .NotNull().WithMessage("{PropertyName} no puede ser nulo")
                    .NotEqual(0).WithMessage("{PropertyName} no puede ser igual a cero"))
                .ChildRules(p => p.RuleFor(p => p.Quantity)
                    .NotNull().WithMessage("{PropertyName} no puede ser nulo")
                    .NotEqual(0).WithMessage("{PropertyName} no puede ser igual a cero"));
        }
    }
}
