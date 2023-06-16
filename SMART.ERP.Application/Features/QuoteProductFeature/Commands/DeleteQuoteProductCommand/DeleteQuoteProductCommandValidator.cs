using FluentValidation;

namespace SMART.ERP.Application.Features.QuoteProductFeature.Commands.DeleteQuoteProductCommand
{
    public class DeleteQuoteProductCommandValidator : AbstractValidator<DeleteQuoteProductCommand>
    {
        public DeleteQuoteProductCommandValidator()
        {
            RuleFor(p => p.Id)
                .NotNull().WithMessage("{PropertyName} no puede ser nulo")
                .NotEqual(0).WithMessage("{PropertyName} no puede ser igual a cero");
        }
    }
}
